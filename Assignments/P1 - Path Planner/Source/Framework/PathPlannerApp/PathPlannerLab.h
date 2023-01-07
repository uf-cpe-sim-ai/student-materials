// Code by Cromwell D. Enage, Jeremiah Blanchard
// October 2010
#pragma once

// change this to start the program on whatever default map as you like from the table below
#define USEDEFAULTMAP hex035x035

#define hex006x006 "./Data/hex006x006.txt"
#define hex014x006 "./Data/hex014x006.txt"
#define hex035x035 "./Data/hex035x035.txt"
#define hex054x045 "./Data/hex054x045.txt"
#define hex098x098 "./Data/hex098x098.txt"
#define hex113x083 "./Data/hex113x083.txt"

// change this to 1(true), and change the data below when you want to test specific starting and goal locations on startup
#define OVERRIDE_DEFAULT_STARTING_DATA 0

// Make sure your start and goal are valid locations!
#define DEFAULT_START_ROW 0
#define DEFAULT_START_COL 0
#define DEFAULT_GOAL_ROW ?
#define DEFAULT_GOAL_COL ?

// Modify the following defines if you have to target a platform prior to the ones specified below.
// Refer to MSDN for the latest info on corresponding values for different platforms.
#ifndef WINVER                  // Allow use of features specific to Windows XP or later.
#define WINVER 0x0501           // Change this to the appropriate value to target other versions of Windows.
#endif

#ifndef _WIN32_WINNT            // Allow use of features specific to Windows XP or later.                   
#define _WIN32_WINNT 0x0501     // Change this to the appropriate value to target other versions of Windows.
#endif

#ifndef _WIN32_WINDOWS          // Allow use of features specific to Windows 98 or later.
#define _WIN32_WINDOWS 0x0410   // Change this to the appropriate value to target Windows Me or later.
#endif

#ifndef _WIN32_IE               // Allow use of features specific to IE 6.0 or later.
#define _WIN32_IE 0x0600        // Change this to the appropriate value to target other versions of IE.
#endif

// Windows Header Files:
#include <windows.h>
#include <commdlg.h>
#include <commctrl.h>

// C RunTime Header Files:
#include <stdlib.h>
#include <malloc.h>
#include <memory.h>
#include <tchar.h>

// Application-Specific Header Files:
#include <fstream>
#include <utility>
#include <vector>
#include "../../PathSearch/PathSearch.h"
#include "../TileSystem/TileMap.h"
#include "Resource/resource.h"

// VC7.1 thinks pre-incrementing inside a while condition is dangerous.
#if _MSC_VER < 1400
#pragma warning(disable : 4288)
#endif

//! \brief Custodian of global variables needed by all classes in this file.
struct PathPlannerGlobals
{
	static const UINT SHOW_SEARCH_RUNNING = 1;

	// friends
	template <typename CharT, typename CharTraits> friend bool loadTimeMapFromStream(std::basic_istream<CharT, CharTraits>& input_stream, ufl_cap4053::TileMap& tile_map);
	friend class GroundUpPathPlanner;
	friend class PathPlannerLab;
private:
	// singleton instance
	static PathPlannerGlobals* instance_;

	// variables
	double min_radius_;
	UINT   flags_;
	ufl_cap4053::Tile* start_tile_ = nullptr;
	ufl_cap4053::Tile* goal_tile_ = nullptr;

	// singleton members
	PathPlannerGlobals();
	PathPlannerGlobals(PathPlannerGlobals const&);
	PathPlannerGlobals& operator=(PathPlannerGlobals const&);
	~PathPlannerGlobals();

	inline void setStartTile(ufl_cap4053::Tile* tile)
	{
		start_tile_ = tile;
	}
	inline void setGoalTile(ufl_cap4053::Tile* tile)
	{
		goal_tile_ = tile;
	}

public:
	static PathPlannerGlobals* getInstance();

	static void deleteInstance();

	inline double getMinTileRadius() const
	{
		return min_radius_;
	}

	inline void setMinTileRadius(double radius)
	{
		min_radius_ = radius;
	}

	inline UINT isFlagOn(UINT flag) const
	{
		return flags_ & flag;
	}

	inline void turnOn(UINT flag)
	{
		flags_ |= flag;
	}

	inline void turnOff(UINT flag)
	{
		flags_ &= ~flag;
	}

	inline ufl_cap4053::Tile* getStartTile()
	{
		return start_tile_;
	}
	inline ufl_cap4053::Tile* getGoalTile()
	{
		return goal_tile_;
	}
};

//! \brief Base interface between the application and the search algorithm.
struct PathPlannerInterface
{
	/*This is the easiest way to get the timed search using the enter and exit code properly... 
	this is because initialize search needs to call the enter for the step and normal run searches*/
	bool								mybTimeSearch = false;
	bool								mybHasEntered = false;
	void setTimedSearch(bool isTimed) {mybTimeSearch = isTimed;}
	PathPlannerInterface() { mybHasEntered  = false; }
	//! \brief This destructor is declared <code>virtual</code> to ensure that the destructors of
	//! derived classes are also invoked.
	virtual ~PathPlannerInterface();

	//! \brief Returns <code>true</code> if it is okay to initialize and run the underlying search
	//! algorithm, <code>false</code> otherwise.
	virtual bool isReady() const = 0;

	//! \brief Resets any parameters that the search algorithm uses.
	virtual void resetParameters() = 0;

	//! \brief Shuts down the search algorithm.
	virtual void shutdownSearch() = 0;

	//! \brief Builds the underlying search algorithm using the contents of the specified input
	//! stream.
	virtual bool read(std::basic_ifstream<TCHAR>& input_stream) = 0;

	virtual void initialize() = 0;

	//! \brief Returns the number of parameters to be displayed by the parameter list view.
	virtual int getInputCount() const = 0;

	//! \brief Displays the relevant parameters and their current values.
	virtual void displayInput(NMLVDISPINFO* list_view_display_info) const = 0;

	//! \brief Updates any parameter values that the user has changed.
	virtual bool updateInput(NMLVDISPINFO const* list_view_display_info) = 0;

	//! \brief Resets the search algorithm.
	virtual void resetSearch() = 0;

	//! \brief Returns <code>TRUE</code> if the "Set Start" button should be enabled,
	//! <code>FALSE</code> otherwise.
	//!
	//! Override only when the search algorithm requires explicit locations as selectable items.
	virtual BOOL shouldEnableSetStart() const;

	//! \brief Updates the index of the start location.
	//!
	//! Override only when the search algorithm requires explicit locations as selectable items.
	virtual bool updateStart();

	//! \brief Returns <code>TRUE</code> if the "Set Goal" button should be enabled,
	//! <code>FALSE</code> otherwise.
	//!
	//! Override only when the search algorithm requires explicit locations.
	virtual BOOL shouldEnableSetGoal() const;

	//! \brief Updates the index of the goal location.
	//!
	//! Override only when the search algorithm requires explicit locations.
	virtual bool updateGoal();

	//! \brief Returns <code>true</code> if the user can press a key to initialize the search
	//! algorithm, <code>false</code> otherwise.
	virtual bool isInitializableByKey() const = 0;

	//! \brief Initializes the underlying search algorithm.
	virtual void initializeSearch() = 0;

	//! \brief Returns <code>true</code> if the user can press a key to run the search algorithm,
	//! <code>false</code> otherwise.
	virtual bool isRunnableByKey() const = 0;

	//! \brief Returns <code>TRUE</code> if the "Run" and "Step" buttons should be enabled,
	//! <code>FALSE</code> otherwise.
	BOOL shouldEnableRun() const;

	//! \brief Runs the search algorithm.
	virtual void runSearch(void/*long long timeslice*/) = 0;

	//! \brief step the search algorithm.
	virtual void stepSearch(void) = 0;

	//! \brief Returns <code>TRUE</code> if the "Time Run" button should be enabled,
	//! <code>FALSE</code> otherwise.
	//!
	//! Override only when the search algorithm has a clear terminating condition that can be
	//! reached during a timed run.
	virtual BOOL shouldEnableTimeRun() const;

	//! \brief Runs the search algorithm to completion and calculates the elapsed time.
	//!
	//! Override only when the search algorithm has a clear terminating condition that can be
	//! reached during a timed run.
	virtual void timeSearch();

	//! \brief Inspects the search algorithm to ensure that it has built the solution properly.
	virtual void checkSolution(HWND window_handle) const;

	//! \brief Shows the current search algortihm at work.
	//!
	//! For a regular search, this function displays the current path, all locations explored, and
	//! any locations that the search algorithm still needs to evaluate.  More exotic algorithms
	//! will exhibit different behavior.
	virtual void displaySearchProgress(POINT const& offset, int width, int height,
	                                   HDC device_context_handle) const = 0;

	//! \brief Returns <code>true</code> by default.
	//!
	//! Derived classes should return <code>false</code> if they override the
	//! <code>beginRedrawSearchProgress()</code> and <code>endRedrawSearchProgress()</code>
	//! methods.
	virtual bool needsFullRedraw() const;

	//! \brief Cleans up potential artifacts from a previous draw.
	//!
	//! Invoked before the search algorithm is executed while the
	//! <code>PathPlannerGlobals::SHOW_SEARCH_RUNNING</code> flag is on.
	virtual void beginRedrawSearchProgress(POINT const& offset, int width, int height,
	                                       HDC device_context_handle) const = 0;

	//! \brief Redraws the progress of the search algorithm.
	//!
	//! Invoked after the search algorithm is executed while the
	//! <code>PathPlannerGlobals::SHOW_SEARCH_RUNNING</code> flag is on.
	//!
	//! Calls <code>displaySearchProgress()</code> by default.  Override if this is inefficient.
	virtual void endRedrawSearchProgress(POINT const& offset, int width, int height,
	                                     HDC device_context_handle) const;
};

//! \brief Tile-based path planner that uses the <code>PathSearch</code> algorithm.
class GroundUpPathPlanner : public PathPlannerInterface
{
	ufl_cap4053::searches::PathSearch			search_;
	ufl_cap4053::TileMap&						tile_map_;
	ufl_cap4053::Tile const*				    start_tile_;
	ufl_cap4053::Tile const*				    goal_tile_;
	LARGE_INTEGER					            frequency_;
	double								        elapsed_time_;
	unsigned int						        iteration_count_;
	int									        start_row_;
	int									        start_column_;
	int									        goal_row_;
	int									        goal_column_;
	unsigned int								myTimeStep;
	unsigned int								myFastTimeStep;
	unsigned int								myNumberofRounds;
	bool										is_initializable_;	
	std::vector<ufl_cap4053::Tile const*>		path2;

public:
	GroundUpPathPlanner(ufl_cap4053::TileMap& tiles);
	bool isReady() const;
	void resetParameters();
	void shutdownSearch();
	bool read(std::basic_ifstream<TCHAR>& input_stream);
	void initialize();
	int getInputCount() const;
	void displayInput(NMLVDISPINFO* list_view_display_info) const;
	bool updateInput(NMLVDISPINFO const* list_view_display_info);
	void resetSearch();
	bool isInitializableByKey() const;
	void initializeSearch();
	bool isRunnableByKey() const;
	void runSearch(void/*long long timeslice*/);
	void stepSearch(void);
	BOOL shouldEnableTimeRun() const;
	void timeSearch();
	void checkSolution(HWND window_handle) const;
	void displaySearchProgress(POINT const& offset, int width, int height,
	                           HDC device_context_handle) const;
	void beginRedrawSearchProgress(POINT const& offset, int width, int height,
	                               HDC device_context_handle) const;
};

//! \brief Police concurrent accesses via critical sections.
class CriticalSectionSynchronizer
{
	CRITICAL_SECTION cs_;

public:
	CriticalSectionSynchronizer();
	~CriticalSectionSynchronizer();
	void acquire();
	void release();
};

//! \brief Police concurrent accesses via mutices.
class MutexSynchronizer
{
	HANDLE mutex_;

public:
	MutexSynchronizer();
	~MutexSynchronizer();
	void acquire();
	void release();
};

class PathPlannerLab
{
	// Pick and choose your concurrent-access police force here.
	typedef MutexSynchronizer Synchronizer;

	static const LONG      MAX_LOADSTRING = 32;
	static const LONG      MAX_BUFFER_SIZE = 256;

	// singleton instance
	static PathPlannerLab* instance_;

	// application handle
	HINSTANCE              application_handle_;

	// list-view column and item counts
	int                    parameter_list_view_item_count_;

	// window, tab, list-view, and button controls
	HWND                   window_handle_;
	HWND                   globals_list_view_handle_;
	HWND                   parameter_list_view_handle_;
	HWND                   open_button_handle_;
	HWND                   reset_button_handle_;
	HWND                   start_waypoint_button_handle_;
	HWND                   goal_waypoint_button_handle_;
	HWND                   run_button_handle_;
	HWND                   step_button_handle_;
	HWND                   time_run_button_handle_;

	// custom windows
	HWND                   tile_grid_handle_;

	// double-buffering primitives
	HDC                    tile_grid_device_context_handle_;
	HDC                    tile_grid_buffer_context_handle_;
	HBITMAP                tile_grid_buffer_bitmap_handle_;

	// run-button icons
	HICON                  pause_icon_handle_;
	HICON                  play_icon_handle_;

	// coordinates, offsets, and dimensions
	int                    tile_grid_x_;
	int                    tile_grid_y_;
	int                    tile_grid_width_;
	int                    tile_grid_height_;
	int                    tile_map_width_;
	int                    tile_map_height_;
	POINT                  tile_grid_offset_;

	// synchronization primitives
	volatile bool          cannot_close_thread_handle_;
	volatile bool          should_stop_thread_handle_;
	volatile bool          needs_full_render_;
	mutable Synchronizer   planner_sync_;
	Synchronizer           thread_sync_;
	DWORD                  thread_id_;
	HANDLE                 thread_handle_;

	// path-planning components
	ufl_cap4053::TileMap   ground_up_tile_map_;
	PathPlannerInterface*  current_planner_;

	// the file filter string
	TCHAR                  text_filter_[12];

	// tooltip strings
	TCHAR                  open_button_text_[14];
	TCHAR                  reset_button_text_[13];
	TCHAR                  start_waypoint_button_text_[10];
	TCHAR                  goal_waypoint_button_text_[9];
	TCHAR                  run_button_text_[4];
	TCHAR                  step_button_text_[5];
	TCHAR                  time_run_button_text_[9];

	// singleton members
	PathPlannerLab();
	PathPlannerLab(PathPlannerLab const&);
	PathPlannerLab& operator=(PathPlannerLab const&);
	~PathPlannerLab();

public:
	static PathPlannerLab* getInstance();
	static void deleteInstance();

	// Needed by the window procedure.
	inline HINSTANCE getApplicationHandle() const
	{
		return application_handle_;
	}

	// Big ol' definition.  Proceed with caution.
	BOOL initializeApplication(HINSTANCE application_handle, int n_cmd_show);

	// Thread callback helper method.
	void threadHandler();

	// Event handlers for push-button presses.
	void onReset();
	void onSetStart();
	void onSetGoal();
	void onRun();
	void onStep();
	void onTimeRun();

	// Event handler for key presses.
	bool onKeyPress(WPARAM w_param);

	// Event handler for list-views.
	LRESULT onListView(LPARAM l_param);

	// Event handler for resizing the tile grid window.
	void onSize(HWND window_handle, WPARAM w_param, LPARAM l_param);

	// Event handler for horizontal scrolling of the tile grid window.
	void onHScroll(HWND window_handle, WPARAM w_param, LPARAM l_param);

	// Event handler for vertical scrolling of the tile grid window.
	void onVScroll(HWND window_handle, WPARAM w_param, LPARAM l_param);

	// Event handler for file opening.
	void onFileOpen(HWND window_handle);

	// Event handler for rendering the path planner.
	void paintTileGrid(HDC device_context_handle) const;

private:
	// Helper methods for updating the UI.
	void resetOffsets_();
	void showRunStopped_();
	void updateRunButtons_();
	void updateButtons_();
	void updateParameterListView_();
	void updateTileGrid_();

	// Helper method for file opening.
	void openFile_(HWND window_handle, TCHAR* file_name);

	// Unsynchronized helper methods for event handlers.
	void reset_();
	bool run_();
	void step_();
	void timeRun_();

	// Helper methods for key presses.
	void resetByKey_();
	void runByKey_();
	void stepByKey_();
	void timeRunByKey_();

	// Helper methods for rendering the path planner.
	void renderFull_();
	void doubleBufferTileGrid_(HDC device_context_handle, HDC buffer_context_handle) const;
	void paintAllTileGrid_(HDC device_context_handle, HDC buffer_context_handle) const;
};

#define CPU_CLOCKS_PER_SEC 10000000
static long long cpu_clock();
