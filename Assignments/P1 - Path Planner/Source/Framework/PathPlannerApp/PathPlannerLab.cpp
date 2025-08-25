// PathPlannerLab.cpp : Defines the entry point for the path planner application.
// Code by Cromwell D. Enage, 2010; Jeremiah Blanchard, 2010, 2012, 2017, 2019
#if defined(_UNICODE) && _MSC_VER < 1400
#error Please use Multi-Byte Character Set with Visual C++ 7.1
#endif

#define _CRT_SECURE_NO_DEPRECATE
#define _CRT_NON_CONFORMING_SWPRINTFS

#include <cassert>
#include <iostream>
#include <cstdio>
#include <cstdlib>
#include <cmath>
#include <cstring>
#include <io.h>
#include <fcntl.h>
#include <algorithm>
#include <deque>
#include <map>
#include <functional>
#include "PathPlannerLab.h"
#include "../TileSystem/TileMapUtility.h"

std::vector<ufl_cap4053::Tile const*> tempSolution;

PathPlannerGlobals* PathPlannerGlobals::instance_ = 0;

PathPlannerGlobals::PathPlannerGlobals() : min_radius_(4.0), flags_(0)
{
}

PathPlannerGlobals::~PathPlannerGlobals()
{
}

PathPlannerGlobals* PathPlannerGlobals::getInstance()
{
	if (instance_ == 0)
	{
		instance_ = new PathPlannerGlobals();
	}

	return instance_;
}

void PathPlannerGlobals::deleteInstance()
{
	if (instance_ != 0)
	{
		delete instance_;
		instance_ = 0;
	}
}

using namespace std;
using namespace ufl_cap4053;
using namespace searches;

void setBounds(double const h_radius, int const row_count, int const column_count,
               POINT const& offset, int grid_width, int grid_height,
               int& row_start, int& row_end, int& column_start, int& column_end)
{
	double const inv_radius = sqrt(3.0) / h_radius;

	row_start = (static_cast<int>(-offset.y * inv_radius) - 1) / 3;
	column_start = (static_cast<int>(-offset.x / h_radius) - 1) >> 1;

	if (column_start < 0)
	{
		column_start = 0;
	}

	row_end = static_cast<int>((grid_height - offset.y) * inv_radius) / 3;

	if (row_count < row_end)
	{
		row_end = row_count;
	}

	column_end = static_cast<int>(((grid_width - offset.x) >> 1) / h_radius);

	if (column_count < column_end)
	{
		column_end = column_count;
	}
}

void constructHexagon(double const radius, POINT const& offset, POINT* hexagon) {
	hexagon[0].x = hexagon[3].x = 0;
	hexagon[4].x = hexagon[5].x = -(hexagon[1].x = hexagon[2].x = static_cast<LONG>(radius));

	if (hexagon[4].x != radius)
	{
		--hexagon[1].x;
		--hexagon[2].x;
		++hexagon[4].x;
		++hexagon[5].x;
	}
	else if (radius < 6.0)
	{
		++hexagon[1].x;
		++hexagon[2].x;
		--hexagon[4].x;
		--hexagon[5].x;
	}

	hexagon[1].y = hexagon[5].y = -(
		hexagon[2].y = hexagon[4].y = static_cast<LONG>(radius / sqrt(3.0))
	);
	hexagon[0].y = -(hexagon[3].y = hexagon[2].y << 1);

	for (unsigned int i = 0; i < 6; ++i)
	{
		hexagon[i].x += offset.x;
		hexagon[i].y += offset.y;
	}
}

void drawTileHighlight(Tile const* tile, POINT const& offset, double const radius, HBRUSH brush_handle, 
					   HDC device_context_handle, double const radius_multiplier = 1) {

	int x_offset = static_cast<int>(tile->getXCoordinate());
	int y_offset = static_cast<int>(tile->getYCoordinate());

	// Note: radius_multiplier can be changed, but has a default value. If it is increased, the highlight will be larger.
	// This is a quickly derived equation for radius growth that maintains as much clarity as possible for large maps.
	// Default = 30/radius + 1.05*radius
	double const calculated_new_radius = ((radius_multiplier * 30) / radius) + ((radius_multiplier * 1.05) * radius);

	POINT end_point_hexagon[6];
	constructHexagon(calculated_new_radius, offset, end_point_hexagon);
	HRGN end_point_hex_region = CreatePolygonRgn(end_point_hexagon, 6, WINDING);

	if (OffsetRgn(end_point_hex_region, x_offset, y_offset) != ERROR) {

		FillRgn(device_context_handle, end_point_hex_region, brush_handle);
		OffsetRgn(end_point_hex_region, -x_offset, -y_offset);
	}
}

void drawGrid(TileMap const& tile_map, POINT const& offset, int grid_width, int grid_height, HDC device_context_handle)
{
	double const radius = tile_map.getTileRadius();
	POINT hexagon[6];
	constructHexagon(radius, offset, hexagon);

	if (HRGN hex_region = CreatePolygonRgn(hexagon, 6, WINDING))
	{
		int row_start;
		int row_end;
		int column_start;
		int column_end;

		setBounds(radius, tile_map.getRowCount(), tile_map.getColumnCount(), offset,
		          grid_width, grid_height, row_start, row_end, column_start, column_end);

		HBRUSH brush_handle[16];
		HBRUSH black_brush_handle = reinterpret_cast<HBRUSH>(GetStockObject(BLACK_BRUSH));
		HBRUSH red_brush_handle = CreateSolidBrush(RGB(255, 0, 0));
		HBRUSH green_brush_handle = CreateSolidBrush(RGB(0, 255, 0));

		unsigned char tile_weight;

		for (unsigned char i = 0; i < 16; ++i)
		{
			tile_weight = 255 - (i << 4);
			brush_handle[i] = CreateSolidBrush(RGB(tile_weight, tile_weight, tile_weight));
		}

		Tile const* tile;
		Tile const* start_tile = PathPlannerGlobals::getInstance()->getStartTile();
		Tile const* goal_tile = PathPlannerGlobals::getInstance()->getGoalTile();
		int x_offset;
		int y_offset;
		int row;
		int column;

		// Draw a red highlight around the start tile (if it exists)
		if (start_tile) {
			drawTileHighlight(start_tile, offset, radius, red_brush_handle, device_context_handle);
		}
		// Draw a green highlight around the goal tile (if it exists)
		if (goal_tile) {
			drawTileHighlight(goal_tile, offset, radius, green_brush_handle, device_context_handle);
		}

		for (row = row_start; row < row_end; ++row)
		{
			for (column = column_start; column < column_end; ++column)
			{
				tile = tile_map.getTile(row, column);
				x_offset = static_cast<int>(tile->getXCoordinate());
				y_offset = static_cast<int>(tile->getYCoordinate());

				if (OffsetRgn(hex_region, x_offset, y_offset) != ERROR)
				{
					if (tile_weight = tile->getWeight())
					{
						if (15 < tile_weight)
						{
							tile_weight = 15;
						}
						FillRgn(device_context_handle, hex_region, brush_handle[tile_weight]);
					}
					else
					{
						FillRgn(device_context_handle, hex_region, black_brush_handle);
					}

					OffsetRgn(hex_region, -x_offset, -y_offset);
				}
			}
		}

		for (unsigned char i = 0; i < 16; ++i)
		{
			DeleteObject(brush_handle[i]);
		}

		DeleteObject(hex_region);
		DeleteObject(black_brush_handle);
		DeleteObject(red_brush_handle);
		DeleteObject(green_brush_handle);
	}
}

void drawEndpoint(int center_x, int center_y, int half_length, HBRUSH brush_handle,
                  HDC device_context_handle)
{
	int x1 = center_x - half_length / 2;
	int x2 = center_x + half_length / 2;
	int y1 = center_y - half_length;
	RECT rectangle;

	rectangle.left = x1 + 1;
	rectangle.right = x2;
	rectangle.top = y1 + 1;
	rectangle.bottom = center_y;
	FillRect(device_context_handle, &rectangle, brush_handle);

	if (MoveToEx(device_context_handle, x1, center_y + half_length, 0))
	{
		LineTo(device_context_handle, x1, y1);
		LineTo(device_context_handle, x2, y1);
		LineTo(device_context_handle, x2, center_y);
		LineTo(device_context_handle, x1, center_y);
	}
}

void displayEndpoints(Tile const* start_tile, Tile const* goal_tile, POINT const& offset,
                      int half_length, HBRUSH start_brush_handle, HBRUSH goal_brush_handle,
                      HDC device_context_handle)
{
	LONG const offset_x = offset.x;
	LONG const offset_y = offset.y;

	drawEndpoint(static_cast<int>(start_tile->getXCoordinate() + offset_x),
	             static_cast<int>(start_tile->getYCoordinate() + offset_y),
	             half_length, start_brush_handle, device_context_handle);
	drawEndpoint(static_cast<int>(goal_tile->getXCoordinate() + offset_x),
	             static_cast<int>(goal_tile->getYCoordinate() + offset_y),
	             half_length, goal_brush_handle, device_context_handle);
}

PathPlannerInterface::~PathPlannerInterface()
{
}

BOOL PathPlannerInterface::shouldEnableSetStart() const
{
	return FALSE;
}

bool PathPlannerInterface::updateStart()
{
	return false;
}

BOOL PathPlannerInterface::shouldEnableSetGoal() const
{
	return FALSE;
}

bool PathPlannerInterface::updateGoal()
{
	return false;
}

BOOL PathPlannerInterface::shouldEnableRun() const
{
	if (isInitializableByKey() || isRunnableByKey())
	{
		return TRUE;
	}
	else
	{
		return FALSE;
	}
}

BOOL PathPlannerInterface::shouldEnableTimeRun() const
{
	return FALSE;
}

void PathPlannerInterface::timeSearch()
{
}

void PathPlannerInterface::checkSolution(HWND window_handle) const
{
}

bool PathPlannerInterface::needsFullRedraw() const
{
	return true;
}

void PathPlannerInterface::endRedrawSearchProgress(POINT const& offset, int width, int height,
                                                   HDC device_context_handle) const
{
	displaySearchProgress(offset, width, height, device_context_handle);
}

GroundUpPathPlanner::GroundUpPathPlanner(TileMap& tiles)
	: search_(), tile_map_(tiles), start_tile_(0), goal_tile_(0), frequency_(), elapsed_time_(0.0)
	, iteration_count_(0), start_row_(0), start_column_(0), goal_row_(0), goal_column_(0), myTimeStep(0), myFastTimeStep(5000), myNumberofRounds(1)
	, is_initializable_(true)
{
	QueryPerformanceFrequency(&frequency_);
}

bool GroundUpPathPlanner::isReady() const
{
	return tile_map_.getRowCount() && tile_map_.getColumnCount();
}

void GroundUpPathPlanner::resetParameters()
{
	start_row_ = start_column_ = goal_row_ = goal_column_ = myTimeStep = 0;
	myFastTimeStep = 5000;
	myNumberofRounds = 1;
	start_tile_ = goal_tile_ = 0;
}

void GroundUpPathPlanner::shutdownSearch()
{
	search_.unload();
}

bool GroundUpPathPlanner::read(basic_ifstream<TCHAR>& input_stream)
{
#if OVERRIDE_DEFAULT_STARTING_DATA 

	start_tile_ = tile_map_.getTile(start_row_ = DEFAULT_START_ROW, start_column_ = DEFAULT_START_COL);
	goal_tile_ = tile_map_.getTile(goal_row_ = DEFAULT_GOAL_ROW,
	                               goal_column_ = DEFAULT_GOAL_COL);

#else
	// These values dictate the default start and goal tile locations
	start_tile_ = tile_map_.getTile(start_row_ = PathPlannerGlobals::getInstance()->getStartTile()->getRow(), 
								   start_column_ = PathPlannerGlobals::getInstance()->getStartTile()->getColumn());
	goal_tile_ = tile_map_.getTile(goal_row_ = PathPlannerGlobals::getInstance()->getGoalTile()->getRow(),
	                               goal_column_ = PathPlannerGlobals::getInstance()->getGoalTile()->getColumn());

#endif
	return true;
}

void GroundUpPathPlanner::initialize()
{
	search_.load(&tile_map_);
}

int GroundUpPathPlanner::getInputCount() const
{
	return 7;
}

void GroundUpPathPlanner::displayInput(NMLVDISPINFO* list_view_display_info) const
{
	LVITEM& item = list_view_display_info->item;

	switch (item.iItem)
	{
		case 0:
		{
			if (item.iSubItem)
			{
				_stprintf(item.pszText, _T("%s"), _T("Start Row"));
			}
			else
			{
				_stprintf(item.pszText, _T("%i"), start_row_);
			}

			break;
		}

		case 1:
		{
			if (item.iSubItem)
			{
				_stprintf(item.pszText, _T("%s"), _T("Start Column"));
			}
			else
			{
				_stprintf(item.pszText, _T("%i"), start_column_);
			}

			break;
		}

		case 2:
		{
			if (item.iSubItem)
			{
				_stprintf(item.pszText, _T("%s"), _T("Goal Row"));
			}
			else
			{
				_stprintf(item.pszText, _T("%i"), goal_row_);
			}

			break;
		}

		case 3:
		{
			if (item.iSubItem)
			{
				_stprintf(item.pszText, _T("%s"), _T("Goal Column"));
			}
			else
			{
				_stprintf(item.pszText, _T("%i"), goal_column_);
			}

			break;
		}

		case 4:
		{
			if (item.iSubItem)
			{
				_stprintf(item.pszText, _T("%s"), _T("Regular Run Time Step(ms)"));
			}
			else
			{
				_stprintf(item.pszText, _T("%i"), myTimeStep);
			}

			break;
		}

		case 5:
		{
			if (item.iSubItem)
			{
				_stprintf(item.pszText, _T("%s"), _T("Fast Run Time Step(ms)"));
			}
			else
			{
				_stprintf(item.pszText, _T("%i"), myFastTimeStep);
			}

			break;
		}

		case 6:
		{
			if (item.iSubItem)
			{
				_stprintf(item.pszText, _T("%s"), _T("Number of rounds"));
			}
			else
			{
				_stprintf(item.pszText, _T("%i"), myNumberofRounds);
			}

			break;
		}
	}
}

bool GroundUpPathPlanner::updateInput(NMLVDISPINFO const* list_view_display_info)
{
	LVITEM const& item = list_view_display_info->item;

	switch (item.iItem)
	{
		case 0:
		{
			int row = 0;

			if (
			    _stscanf(item.pszText, _T("%i"), &row)
			 && (0 <= row)
			 && (row < tile_map_.getRowCount())
			 && tile_map_.getTile(row, start_column_)->getWeight()
			)
			{
				start_row_ = tile_map_.getRowCount();

				if (row < start_row_)
				{
					start_row_ = row;
				}
				else
				{
					--start_row_;
				}

				start_tile_ = tile_map_.getTile(start_row_, start_column_);
				PathPlannerGlobals::getInstance()->setStartTile(tile_map_.getTile(start_row_, start_column_));
				return true;
			}

			break;
		}

		case 1:
		{
			int column = 0;

			if (
			    _stscanf(item.pszText, _T("%i"), &column)
			 && (0 <= column)
			 && (column < tile_map_.getColumnCount())
			 && tile_map_.getTile(start_row_, column)->getWeight()
			)
			{
				start_column_ = tile_map_.getColumnCount();

				if (column < start_column_)
				{
					start_column_ = column;
				}
				else
				{
					--start_column_;
				}

				start_tile_ = tile_map_.getTile(start_row_, start_column_);
				PathPlannerGlobals::getInstance()->setStartTile(tile_map_.getTile(start_row_, start_column_));
				return true;
			}

			break;
		}

		case 2:
		{
			int row = 0;

			if (
			    _stscanf(item.pszText, _T("%i"), &row)
			 && (0 <= row)
			 && (row < tile_map_.getRowCount())
			 && tile_map_.getTile(row, goal_column_)->getWeight()
			)
			{
				goal_row_ = tile_map_.getRowCount();

				if (row < goal_row_)
				{
					goal_row_ = row;
				}
				else
				{
					--goal_row_;
				}

				goal_tile_ = tile_map_.getTile(goal_row_, goal_column_);
				PathPlannerGlobals::getInstance()->setGoalTile(tile_map_.getTile(goal_row_, goal_column_));
				return true;
			}

			break;
		}

		case 3:
		{
			int column = 0;

			if (
			    _stscanf(item.pszText, _T("%i"), &column)
			 && (0 <= column)
			 && (column < tile_map_.getColumnCount())
			 && tile_map_.getTile(goal_row_, column)->getWeight()
			)
			{
				goal_column_ = tile_map_.getColumnCount();

				if (column < goal_column_)
				{
					goal_column_ = column;
				}
				else
				{
					--goal_column_;
				}

				goal_tile_ = tile_map_.getTile(goal_row_, goal_column_);
				PathPlannerGlobals::getInstance()->setGoalTile(tile_map_.getTile(goal_row_, goal_column_));
				return true;
			}

			break;
		}

		case 4:
			{
				int result = _stscanf(item.pszText, _T("%i"), &myTimeStep);
					break;
			}

		case 5:
			{
				int result = _stscanf(item.pszText, _T("%i"), &myFastTimeStep);
				break;
			}

		case 6:
			{
				int result = _stscanf(item.pszText, _T("%i"), &myNumberofRounds);
				break;
			}
	}

	return false;
}

void GroundUpPathPlanner::resetSearch()
{
	if(mybHasEntered)
		search_.shutdown();

		tempSolution.clear();

	tile_map_.resetTileDrawing();
	elapsed_time_ = 0.0;
	iteration_count_ = 0;
	is_initializable_ = true;
	mybHasEntered = false;
}

bool GroundUpPathPlanner::isInitializableByKey() const
{
	return is_initializable_;
}

void GroundUpPathPlanner::initializeSearch()
{
	elapsed_time_ = 0.0;
	iteration_count_ = 0;
	if(!mybTimeSearch)
	{
		search_.initialize(start_row_, start_column_, goal_row_, goal_column_);
		mybHasEntered  = true;
	}
	is_initializable_ = false;

}

bool GroundUpPathPlanner::isRunnableByKey() const
{
	return !is_initializable_ && !search_.isDone()/*search_.getSolution().empty()*/;
}

void GroundUpPathPlanner::runSearch(void/*long long timeslice*/)
{
	if(!search_.isDone())
	{
		search_.update(static_cast<long>(myTimeStep/*timeslice*/));
		++iteration_count_;
	}
}

void GroundUpPathPlanner::stepSearch(void)
{
	if(!search_.isDone())
	{
		search_.update(static_cast<long>(0/*timeslice*/));
		++iteration_count_;
	}
}

BOOL GroundUpPathPlanner::shouldEnableTimeRun() const
{
	return isReady() ? TRUE : FALSE;
}

void GroundUpPathPlanner::timeSearch()
{
	long long startTime, endTime;

	mybTimeSearch = true;

	initializeSearch();

	tempSolution.clear();
	mybHasEntered = true;
	startTime = cpu_clock();


	startTime = cpu_clock();
	for(unsigned int i = 0; i < myNumberofRounds; i++)
	{
		search_.initialize(start_row_, start_column_, goal_row_, goal_column_);
		search_.update(myFastTimeStep);//Run for this long or else
		tempSolution = search_.getSolution();//don't like this but it solves functionality
		search_.shutdown();
	}
	endTime = cpu_clock();
	mybHasEntered = false;

	elapsed_time_ = (double)(endTime - startTime) / CPU_CLOCKS_PER_SEC;
	std::cout << "average elapsed time: " << elapsed_time_ / (double)myNumberofRounds << " sec" << std::endl;

	if(search_.isDone())
	{
		if(tempSolution.size())
		{
			if (tempSolution.back() != start_tile_)
			{
				MessageBox(NULL,
					_T("The first tile is not the start!"),
					_T("Solution Checker"), MB_OK);
			}

			if (tempSolution.front() != goal_tile_)
			{
				MessageBox(NULL, _T("The last tile is not the goal!"),
					_T("Solution Checker"), MB_OK);
			}

			//deque<double> v1, v2;
			std::size_t solutionsize = tempSolution.size() - 1;
			//double dare = tile_map_.getTileRadius() * (1 << 1);
			//double down = dare * dare;

			//dare = down + 0.00001;
			//down -= 0.00001;

			unsigned int i = 0;
			while (i < solutionsize)
			{
				// Check from beginning to end, each node against another next to it in the list to see if they are next to each other.
				const Tile * tile1 = tempSolution[i];
				const Tile * tile2 = tempSolution[i+1];

				double deltaX = tile1->getXCoordinate() - tile2->getXCoordinate();
				double deltaY = tile1->getYCoordinate() - tile2->getYCoordinate();

				double distance = sqrt((deltaX * deltaX) + (deltaY * deltaY));

				double studentTileDistance = distance - .0001f; // Making sure we take into account of floatation error

				double actualTtileDistance = tile_map_.getTileRadius() * 2;
				if (tile1->getRow() == tile2->getRow() && tile1->getColumn() == tile2->getColumn())
				{
					MessageBox(NULL, _T("A tile occurs twice (sequentially) in the returned path!"),
						_T("Solution Checker"), MB_OK);
				}
				if (studentTileDistance > actualTtileDistance)
				{
					MessageBox(NULL, _T("A tile is not adjacent to its predecessor!"),
						_T("Solution Checker"), MB_OK);
				}
				i++;

				//v1.clear();
				//v2.clear();
				//v1.push_back(tempSolution[i - 1]->getYCoordinate());
				//v2.push_front(tempSolution[i]->getXCoordinate());
				//v1.push_back(tempSolution[i - 1]->getXCoordinate());
				//v2.push_front(tempSolution[i]->getYCoordinate());
				//transform(v1.begin(), v1.end(), v2.begin(), v1.begin(), minus<double>());
				//transform(v1.begin(), v1.end(), v2.begin(), bind2nd(ptr_fun<double,double>(pow), 2.0));
				//v1.back() = v2.back() + v2.front();

				//if (dare < v1.back() || v1.back() < down)
				//{
				//	MessageBox(NULL, _T("A node is not next to its parent!"),
				//		_T("Solution Checker"), MB_OK);
				//}
			}
		}
		else/* if(!mybTimeSearch) *///!done
		{
			MessageBox(NULL,
				_T("isDone() returned true and getSolution() returned a vector size 0"),
				_T("Solution Checker"), MB_OK);
		}
	}
	else
	{
		MessageBox(NULL,
			_T("Either Update() returned without isDone() returning true \n            or           \nUpdate() is taking way too long!"),
			_T("Solution Checker"), MB_OK);
		resetSearch();
	}

}

void GroundUpPathPlanner::checkSolution(HWND window_handle) const
{
	if (search_.isDone())
	{
		tempSolution = search_.getSolution();
		if(tempSolution.size())
		{
			if (tempSolution.back() != start_tile_)
			{
				MessageBox(window_handle,
					_T("The first tile is not the start!"),
					_T("Solution Checker"), MB_OK);
			}

			if (tempSolution.front() != goal_tile_)
			{
				MessageBox(window_handle, _T("The last tile is not the goal!"),
					_T("Solution Checker"), MB_OK);
			}
			std::size_t solutionsize = tempSolution.size() - 1;
			unsigned int i = 0;
			while (i < solutionsize)
			{
				const Tile * tile1 = tempSolution[i];
				const Tile * tile2 = tempSolution[i + 1];

				double deltaX = tile1->getXCoordinate() - tile2->getXCoordinate();
				double deltaY = tile1->getYCoordinate() - tile2->getYCoordinate();

				double distance = sqrt((deltaX * deltaX) + (deltaY * deltaY));

				double studentTileDistance = distance - .0001f; // Making sure we take into account of floatation error

				double actualTtileDistance = tile_map_.getTileRadius() * 2;
				if (studentTileDistance > actualTtileDistance)
				{
					MessageBox(NULL, _T("A node is not next to its parent!"),
						_T("Solution Checker"), MB_OK);
				}
				i++;
			}
		}
		else/* if(!mybTimeSearch) *///!done
		{
			MessageBox(NULL,
				_T("isDone() returned true and getSolution() returned a vector size 0"),
				_T("Solution Checker"), MB_OK);
		}
	}
	
}

void GroundUpPathPlanner::displaySearchProgress(POINT const& offset, int width, int height,
                                                HDC device_context_handle) const
{
	if (!isReady())
	{
		return;
	}

	double const tile_radius = tile_map_.getTileRadius();
	int large_node_radius = static_cast<int>(tile_radius * 0.75);
	HBRUSH start_brush_handle = CreateSolidBrush(RGB(255, 0, 0));
	HBRUSH goal_brush_handle = CreateSolidBrush(RGB(0, 255, 0));

	Tile const* tile;
	int x, y, column;

	for (int row = 0; row < tile_map_.getRowCount(); ++row)
	{
		for (column = 0; column < tile_map_.getColumnCount(); ++column)
		{
			tile = tile_map_.getTile(row, column);
			x = static_cast<int>(offset.x + tile->getXCoordinate());
			y = static_cast<int>(offset.y + tile->getYCoordinate());

			if (unsigned int fill = tile->getFill())
			{
				int const radius = static_cast<int>(tile_radius * 0.75);
				unsigned int outline = tile->getOutline();
				HPEN outline_pen_handle
					= outline
					? CreatePen(PS_SOLID, 2, static_cast<COLORREF>(outline))
					: 0;
				HGDIOBJ old_pen_handle
					= outline_pen_handle
					? SelectObject(device_context_handle, outline_pen_handle)
					: 0;
				HBRUSH fill_brush_handle = CreateSolidBrush(static_cast<COLORREF>(fill));
				HGDIOBJ old_brush_handle = SelectObject(device_context_handle, fill_brush_handle);

				BeginPath(device_context_handle);
				MoveToEx(device_context_handle, x + radius, y, 0);
				AngleArc(device_context_handle, x, y, radius, 0.0f, 360.0f);
				EndPath(device_context_handle);
				StrokeAndFillPath(device_context_handle);
				SelectObject(device_context_handle, old_brush_handle);
				DeleteObject(fill_brush_handle);

				if (outline_pen_handle)
				{
					SelectObject(device_context_handle, old_pen_handle);
					DeleteObject(outline_pen_handle);
				}
			}

			if (unsigned int marker = tile->getMarker())
			{
				int const radius = static_cast<int>(tile_radius * 0.5);
				HBRUSH marker_brush_handle = CreateSolidBrush(static_cast<COLORREF>(marker));
				HGDIOBJ old_brush_handle = SelectObject(
				    device_context_handle
				  , marker_brush_handle
				);

				BeginPath(device_context_handle);
				MoveToEx(device_context_handle, x + radius, y, 0);
				AngleArc(device_context_handle, x, y, radius, 0.0f, 360.0f);
				EndPath(device_context_handle);
				StrokeAndFillPath(device_context_handle);
				SelectObject(device_context_handle, old_brush_handle);
				DeleteObject(marker_brush_handle);
			}
		}
	}

	if (/*!path.empty()*/search_.isDone())
	{
		vector<Tile const*> const& path = tempSolution;
		if(path.size())
		{
			RECT rectangle;
			int small_node_radius = static_cast<int>(tile_radius * 0.5);
			HBRUSH path_brush_handle = CreateSolidBrush(RGB(127, 255, 127));
			HGDIOBJ old_brush_handle = SelectObject(device_context_handle, path_brush_handle);

			for (size_t index = 0; index < path.size(); ++index)
			{
				tile = path[index];
				x = static_cast<int>(offset.x + tile->getXCoordinate());
				y = static_cast<int>(offset.y + tile->getYCoordinate());
				rectangle.left = x - small_node_radius;
				rectangle.right = x + small_node_radius;
				rectangle.top = y - small_node_radius;
				rectangle.bottom = y + small_node_radius;
				FillRect(device_context_handle, &rectangle, path_brush_handle);
			}

			SelectObject(device_context_handle, old_brush_handle);
			DeleteObject(path_brush_handle);
		}

		/*}
		else
		{
			MessageBox(NULL,
				_T("isDone() returned true and getSolution() returned a vector size 0"),
				_T("Solution Checker"), MB_OK);
		}*/
	}

	for (int row = 0; row < tile_map_.getRowCount(); ++row)
	{
		for (column = 0; column < tile_map_.getColumnCount(); ++column)
		{
			tile = tile_map_.getTile(row, column);
			
			//Get the head of the singly linked list
			const std::vector<pair<const Tile*, unsigned>> lines = tile->getLines();
			
			for (auto lineSet : lines)
			{
				const Tile* destination = lineSet.first;
				unsigned lineColor = lineSet.second;

				x = static_cast<int>(offset.x + tile->getXCoordinate());
				y = static_cast<int>(offset.y + tile->getYCoordinate());

				HPEN line_pen_handle = CreatePen(
					PS_SOLID
					, 3
					, static_cast<COLORREF>(lineColor)
					);
				HGDIOBJ old_pen_handle = SelectObject(device_context_handle, line_pen_handle);

				MoveToEx(device_context_handle, x, y, 0);
				x = static_cast<int>(offset.x + destination->getXCoordinate());
				y = static_cast<int>(offset.y + destination->getYCoordinate());
				LineTo(device_context_handle, x, y);
				SelectObject(device_context_handle, old_pen_handle);
				DeleteObject(line_pen_handle);
			}
		}
	}

	displayEndpoints(start_tile_, goal_tile_, offset, large_node_radius,
		start_brush_handle, goal_brush_handle, device_context_handle);
	DeleteObject(goal_brush_handle);
	DeleteObject(start_brush_handle);
}

void GroundUpPathPlanner::beginRedrawSearchProgress(POINT const& offset, int width, int height,
                                                    HDC device_context_handle) const
{
	drawGrid(tile_map_, offset, width, height, device_context_handle);
}

BOOL onOkCancelCommand(HWND dialog_handle, WPARAM w_param)
{
	switch (LOWORD(w_param))
	{
		case IDOK:
		case IDCANCEL:
		{
			EndDialog(dialog_handle, LOWORD(w_param));
			return TRUE;
		}
	}

	return FALSE;
}

// Message handler for about box.
INT_PTR CALLBACK aboutDialog(HWND dialog_handle, UINT message, WPARAM w_param, LPARAM l_param)
{
	UNREFERENCED_PARAMETER(l_param);

	switch (message)
	{
		case WM_INITDIALOG:
		{
			return static_cast<INT_PTR>(TRUE);
		}

		case WM_COMMAND:
		{
			return static_cast<INT_PTR>(onOkCancelCommand(dialog_handle, w_param));
		}
	}

	return static_cast<INT_PTR>(FALSE);
}

// Message handler for main window.
LRESULT CALLBACK windowProcedure(HWND window_handle, UINT message, WPARAM w_param, LPARAM l_param)
{
	switch (message)
	{
		case WM_COMMAND:
		{
			// Parse the menu selections:
			switch (LOWORD(w_param))
			{
				case IDM_APP_OPEN:
				{
					PathPlannerLab::getInstance()->onFileOpen(window_handle);
					break;
				}

				case IDM_APP_EXIT:
				{
					DestroyWindow(window_handle);
					break;
				}

				case IDM_APP_ABOUT:
				{
					DialogBox(PathPlannerLab::getInstance()->getApplicationHandle(),
					          MAKEINTRESOURCE(IDD_ABOUTBOX), window_handle, aboutDialog);
					break;
				}

				case IDM_PATHPLANNER_RESET:
				{
					PathPlannerLab::getInstance()->onReset();
					break;
				}

				case IDM_PATHPLANNER_SET_START:
				{
					PathPlannerLab::getInstance()->onSetStart();
					break;
				}

				case IDM_PATHPLANNER_SET_GOAL:
				{
					PathPlannerLab::getInstance()->onSetGoal();
					break;
				}

				case IDM_PATHPLANNER_RUN:
				{
					PathPlannerLab::getInstance()->onRun();
					break;
				}

				case IDM_PATHPLANNER_STEP:
				{
					PathPlannerLab::getInstance()->onStep();
					break;
				}

				case IDM_PATHPLANNER_TIME_RUN:
				{
					PathPlannerLab::getInstance()->onTimeRun();
					break;
				}
			}

			break;
		}

		case WM_NOTIFY:
		{
			return PathPlannerLab::getInstance()->onListView(l_param);
		}

		case WM_KEYDOWN:
		{
			if (PathPlannerLab::getInstance()->onKeyPress(w_param))
			{
				return 0;
			}
			else
			{
				break;
			}
		}

		case WM_DESTROY:
		{
			PostQuitMessage(0);
			return 0;
		}
	}

	return DefWindowProc(window_handle, message, w_param, l_param);
}

// Message handler for tile grid window.
LRESULT CALLBACK tileGridProcedure(HWND window_handle, UINT message,
                                   WPARAM w_param, LPARAM l_param)
{
	switch (message)
	{
		case WM_KEYDOWN:
		{
			if (PathPlannerLab::getInstance()->onKeyPress(w_param))
			{
				return 0;
			}
			else
			{
				break;
			}
		}

		case WM_PAINT:
		{
			PAINTSTRUCT ps;

			PathPlannerLab::getInstance()->paintTileGrid(BeginPaint(window_handle, &ps));
			EndPaint(window_handle, &ps);
			return 0;
		}

		case WM_SIZE:
		{
			PathPlannerLab::getInstance()->onSize(window_handle, w_param, l_param);
			return 0;
		}

		case WM_HSCROLL:
		{
			PathPlannerLab::getInstance()->onHScroll(window_handle, w_param, l_param);
			return 0;
		}

		case WM_VSCROLL:
		{
			PathPlannerLab::getInstance()->onVScroll(window_handle, w_param, l_param);
			return 0;
		}
	}

	return DefWindowProc(window_handle, message, w_param, l_param);
}

DWORD APIENTRY plannerThread(LPVOID unused)
{
	PathPlannerLab::getInstance()->threadHandler();
	return 0;
}

CriticalSectionSynchronizer::CriticalSectionSynchronizer() : cs_()
{
	InitializeCriticalSection(&cs_);
}

CriticalSectionSynchronizer::~CriticalSectionSynchronizer()
{
	DeleteCriticalSection(&cs_);
}

void CriticalSectionSynchronizer::acquire()
{
	EnterCriticalSection(&cs_);
}

void CriticalSectionSynchronizer::release()
{
	LeaveCriticalSection(&cs_);
}

MutexSynchronizer::MutexSynchronizer() : mutex_(CreateMutex(0, FALSE, 0))
{
}

MutexSynchronizer::~MutexSynchronizer()
{
}

void MutexSynchronizer::acquire()
{
	DWORD wait_result;

	do
	{
		wait_result = WaitForSingleObject(mutex_, INFINITE);
	}
	while (!(wait_result == WAIT_OBJECT_0) && !(wait_result == WAIT_ABANDONED));
}

void MutexSynchronizer::release()
{
	ReleaseMutex(mutex_);
}

PathPlannerLab* PathPlannerLab::instance_ = NULL;

PathPlannerLab::PathPlannerLab()
	: application_handle_(0)
	, parameter_list_view_item_count_(0)
	, window_handle_(0)
	, globals_list_view_handle_(0)
	, parameter_list_view_handle_(0)
	, open_button_handle_(0)
	, reset_button_handle_(0)
	, start_waypoint_button_handle_(0)
	, goal_waypoint_button_handle_(0)
	, run_button_handle_(0)
	, step_button_handle_(0)
	, time_run_button_handle_(0)
	, tile_grid_handle_(0)
	, tile_grid_device_context_handle_(0)
	, tile_grid_buffer_context_handle_(0)
	, tile_grid_buffer_bitmap_handle_(0)
	, pause_icon_handle_(0)
	, play_icon_handle_(0)
	, tile_grid_x_(0)
	, tile_grid_y_(40)
	, tile_grid_width_(810)
	, tile_grid_height_(0)
	, tile_map_width_(0)
	, tile_map_height_(0)
	, tile_grid_offset_()
	, cannot_close_thread_handle_(true)
	, should_stop_thread_handle_(false)
	, needs_full_render_(false)
	, planner_sync_()
	, thread_sync_()
	, thread_id_(0)
	, thread_handle_(CreateThread(0, 0, plannerThread, 0, CREATE_SUSPENDED, &thread_id_))
	, ground_up_tile_map_()
	, current_planner_(new GroundUpPathPlanner(ground_up_tile_map_))
{
	resetOffsets_();

	// This is a FOB's way to store a file filter string.
	text_filter_[0] = 'T';
	text_filter_[1] = 'e';
	text_filter_[2] = 'x';
	text_filter_[3] = 't';
	text_filter_[4] = '\0';
	text_filter_[5] = '*';
	text_filter_[6] = '.';
	text_filter_[7] = 'T';
	text_filter_[8] = 'X';
	text_filter_[9] = 'T';
	text_filter_[10] = '\0';
	text_filter_[11] = '\0';

	// Initialize the tooltip strings.
	_stprintf(open_button_text_, _T("%s"), _T("Open Tile Map"));
	_stprintf(reset_button_text_, _T("%s"), _T("Reset Search"));
	_stprintf(start_waypoint_button_text_, _T("%s"), _T("Set Start"));
	_stprintf(goal_waypoint_button_text_, _T("%s"), _T("Set Goal"));
	_stprintf(run_button_text_, _T("%s"), _T("Run"));
	_stprintf(step_button_text_, _T("%s"), _T("Step"));
	_stprintf(time_run_button_text_, _T("%s"), _T("Time Run"));
}

PathPlannerLab::~PathPlannerLab()
{
	thread_sync_.acquire();
	should_stop_thread_handle_ = true;
	thread_sync_.release();

	while (cannot_close_thread_handle_)
	{
		Sleep(100);
	}

	DeleteObject(tile_grid_buffer_bitmap_handle_);
	DeleteDC(tile_grid_buffer_context_handle_);
	ReleaseDC(window_handle_, tile_grid_device_context_handle_);
	CloseHandle(thread_handle_);
	thread_handle_ = 0;
	thread_id_ = 0;
	current_planner_->shutdownSearch();
	delete current_planner_;
}

PathPlannerLab* PathPlannerLab::getInstance()
{
	if (instance_ == 0)
	{
		instance_ = new PathPlannerLab();
	}

	return instance_;
}

void PathPlannerLab::deleteInstance()
{
	if (instance_ != 0)
	{
		delete instance_;
		instance_ = 0;
	}
}

void PathPlannerLab::resetOffsets_()
{
	tile_grid_offset_.x = tile_grid_offset_.y = 0;
}

void PathPlannerLab::showRunStopped_()
{
	SendMessage(run_button_handle_, BM_SETIMAGE, IMAGE_ICON,
	            reinterpret_cast<LPARAM>(play_icon_handle_));
}

void PathPlannerLab::updateRunButtons_()
{
	BOOL should_enable = (
		tile_map_width_ && tile_map_height_
	) ? current_planner_->shouldEnableRun() : FALSE;

	EnableWindow(run_button_handle_, should_enable);
	EnableWindow(step_button_handle_, should_enable);
}

void PathPlannerLab::updateButtons_()
{
	if (tile_map_width_ && tile_map_height_)
	{
		EnableWindow(reset_button_handle_, TRUE);
		EnableWindow(start_waypoint_button_handle_, current_planner_->shouldEnableSetStart());
		EnableWindow(goal_waypoint_button_handle_, current_planner_->shouldEnableSetGoal());
		EnableWindow(time_run_button_handle_, current_planner_->shouldEnableTimeRun());
	}
	else
	{
		EnableWindow(reset_button_handle_, FALSE);
		EnableWindow(start_waypoint_button_handle_, FALSE);
		EnableWindow(goal_waypoint_button_handle_, FALSE);
		EnableWindow(time_run_button_handle_, FALSE);
	}

	updateRunButtons_();
}

void PathPlannerLab::updateParameterListView_()
{
	LVITEM list_view_item;
	int new_count = current_planner_->getInputCount();

	while (new_count < parameter_list_view_item_count_)
	{
		ListView_DeleteItem(parameter_list_view_handle_, --parameter_list_view_item_count_);
	}

	list_view_item.mask = LVIF_TEXT;
	list_view_item.pszText = LPSTR_TEXTCALLBACK;

	while (parameter_list_view_item_count_ < new_count)
	{
		list_view_item.iItem = parameter_list_view_item_count_;
		list_view_item.iSubItem = 0;
		ListView_InsertItem(parameter_list_view_handle_, &list_view_item);
		list_view_item.iSubItem = 1;
		SendMessage(parameter_list_view_handle_, LVM_SETITEM, 0,
		            reinterpret_cast<LPARAM>(&list_view_item));
		++parameter_list_view_item_count_;
	}

	InvalidateRect(parameter_list_view_handle_, 0, TRUE);
}

void PathPlannerLab::updateTileGrid_()
{
	resetOffsets_();

	SCROLLINFO scroll_info;

	scroll_info.cbSize = sizeof(SCROLLINFO);
	scroll_info.fMask  = SIF_POS;
	scroll_info.nPos   = 0;
	MoveWindow(tile_grid_handle_, 0, 0, 0, 0, FALSE);
	MoveWindow(
	    tile_grid_handle_
	  , tile_grid_x_
	  , tile_grid_y_
	  , tile_grid_width_
	  , tile_grid_height_
	  , FALSE
	);
	SetScrollInfo(tile_grid_handle_, SB_HORZ, &scroll_info, TRUE);
	SetScrollInfo(tile_grid_handle_, SB_VERT, &scroll_info, TRUE);
	InvalidateRect(tile_grid_handle_, 0, FALSE);
}

void PathPlannerLab::openFile_(HWND window_handle, TCHAR* file_name)
{
#if defined(_UNICODE) && _MSC_VER < 1400
	char file_name_2003[MAX_BUFFER_SIZE];

	for (size_t i = 0; i < MAX_BUFFER_SIZE; ++i)
	{
		file_name_2003[i] = static_cast<char>(file_name[i]);
	}

	basic_ifstream<TCHAR> input_file_stream(file_name_2003);
#else
	basic_ifstream<TCHAR> input_file_stream(file_name);
#endif

	if (input_file_stream.good())
	{
		planner_sync_.acquire();
		PathPlannerGlobals::getInstance()->turnOff(PathPlannerGlobals::SHOW_SEARCH_RUNNING);
		current_planner_->resetSearch();
		tempSolution.clear();

		bool has_read = loadTileMapFromStream(input_file_stream, ground_up_tile_map_);
		PathPlannerGlobals::getInstance()->setGoalTile(ground_up_tile_map_.getGoalTile());
		PathPlannerGlobals::getInstance()->setStartTile(ground_up_tile_map_.getStartTile());

		if (has_read)
		{
			current_planner_->shutdownSearch();
			has_read = has_read && current_planner_->read(input_file_stream);
		}

		if (has_read)
		{
			int column_count = ground_up_tile_map_.getColumnCount();
			// VISUAL HACK.
			double h_radius = ((column_count < 20) ? 354.0 : 372.0) / column_count;
			double min_radius = PathPlannerGlobals::getInstance()->getMinTileRadius();

			if (h_radius < min_radius)
			{
				h_radius = min_radius;
			}

			int row_count = ground_up_tile_map_.getRowCount();
			// VISUAL HACK.
			double v_radius = ((row_count < 20) ? 338.0 : 360.0) / row_count;

			if (v_radius < min_radius)
			{
				v_radius = min_radius;
			}

			min_radius = (h_radius < v_radius) ? h_radius : v_radius;
			ground_up_tile_map_.setRadius(min_radius);

			long long startTime = cpu_clock();
			current_planner_->initialize();
			double duration = (double) (cpu_clock() - startTime) / CPU_CLOCKS_PER_SEC;
			std::cout << "load time: " << duration << " sec" << std::endl;

			tile_map_width_ = static_cast<int>(((column_count << 1) | 1) * min_radius);
			tile_map_height_ = static_cast<int>(((row_count * 3) + 1) * min_radius / sqrt(3.0));
		}
		else
		{
			current_planner_->resetParameters();
			ground_up_tile_map_.reset();
			tile_map_width_ = tile_map_height_ = 0;
		}

		planner_sync_.release();
		showRunStopped_();
		updateButtons_();

		if (!has_read)
		{
			MessageBox(window_handle, _T("Invalid file."), _T("IO Error"), MB_OK);
		}

		updateParameterListView_();
		updateTileGrid_();
		input_file_stream.close();
		input_file_stream.clear();
	}
}

BOOL PathPlannerLab::initializeApplication(HINSTANCE application_handle, int n_cmd_show)
{
	TCHAR buffer[MAX_LOADSTRING];
	TCHAR app_class_string[MAX_LOADSTRING];

	INITCOMMONCONTROLSEX init_common_controls;

	init_common_controls.dwSize = sizeof(INITCOMMONCONTROLSEX);
	init_common_controls.dwICC = ICC_LISTVIEW_CLASSES | ICC_TAB_CLASSES;
	InitCommonControlsEx(&init_common_controls);

	LoadString(application_handle, IDS_APP_TITLE, buffer, MAX_LOADSTRING);

	WNDCLASSEX window_class_ex;

	window_class_ex.cbSize = sizeof(WNDCLASSEX);
	window_class_ex.style = 0;
	window_class_ex.lpfnWndProc = windowProcedure;
	window_class_ex.cbClsExtra = 0;
	window_class_ex.cbWndExtra = 0;
	window_class_ex.hInstance = application_handle_ = application_handle;
	window_class_ex.hIcon = LoadIcon(application_handle, MAKEINTRESOURCE(IDI_UFL_CAP4053));
	window_class_ex.hCursor = LoadCursor(0, IDC_ARROW);
	window_class_ex.hbrBackground = reinterpret_cast<HBRUSH>(GetStockObject(BLACK_BRUSH));
	window_class_ex.lpszMenuName = MAKEINTRESOURCE(IDC_PATHPLANNER);
	window_class_ex.lpszClassName = app_class_string;
	window_class_ex.hIconSm = LoadIcon(application_handle, MAKEINTRESOURCE(IDI_UFL_CAP4053));
	LoadString(application_handle, IDC_PATHPLANNER, app_class_string, MAX_LOADSTRING);
	RegisterClassEx(&window_class_ex);

	if (
		window_handle_ = CreateWindow(
		    app_class_string
		  , buffer
		  , (
		        WS_OVERLAPPED
		      | WS_CAPTION
		      | WS_SYSMENU
		      | WS_MINIMIZEBOX
		    )
		  , CW_USEDEFAULT
		  , CW_USEDEFAULT
		  , 1080
		  , 800
		  , 0
		  , 0
		  , application_handle
		  , 0
		)
	)
	{
		LONG const y_offset_1 = 0;
		RECT client_rectangle;

		GetClientRect(window_handle_, &client_rectangle);
		client_rectangle.right -= client_rectangle.left;
		client_rectangle.bottom -= client_rectangle.top;
		client_rectangle.left = client_rectangle.top = 0;
		tile_grid_height_ = client_rectangle.bottom - tile_grid_y_;

		LONG const height_0 = tile_grid_y_ - y_offset_1;
		HICON icon_handle;

		if (
			open_button_handle_ = CreateWindow(
			    _T("BUTTON")
			  , open_button_text_
			  , WS_TABSTOP | WS_CHILD | WS_VISIBLE | BS_PUSHBUTTON | BS_ICON
			  , 0
			  , y_offset_1
			  , height_0
			  , height_0
			  , window_handle_
			  , reinterpret_cast<HMENU>(IDM_APP_OPEN)
			  , application_handle
			  , 0
			)
		)
		{
			icon_handle = LoadIcon(application_handle, MAKEINTRESOURCE(IDI_OPEN));
			SendMessage(open_button_handle_, BM_SETIMAGE, IMAGE_ICON,
			            reinterpret_cast<LPARAM>(icon_handle));
			EnableWindow(open_button_handle_, TRUE);
		}
		else
		{
			MessageBox(window_handle_, _T("Creation Failed"), _T("open_button_handle_"), MB_OK);
			DestroyWindow(window_handle_);
			return FALSE;
		}

		if (
			reset_button_handle_ = CreateWindow(
			    _T("BUTTON")
			  , reset_button_text_
			  , WS_TABSTOP | WS_CHILD | WS_VISIBLE | BS_PUSHBUTTON | BS_ICON
			  , height_0
			  , y_offset_1
			  , height_0
			  , height_0
			  , window_handle_
			  , reinterpret_cast<HMENU>(IDM_PATHPLANNER_RESET)
			  , application_handle
			  , 0
			)
		)
		{
			icon_handle = LoadIcon(application_handle, MAKEINTRESOURCE(IDI_STOP));
			SendMessage(reset_button_handle_, BM_SETIMAGE, IMAGE_ICON,
			            reinterpret_cast<LPARAM>(icon_handle));
			EnableWindow(reset_button_handle_, TRUE);
		}
		else
		{
			MessageBox(window_handle_, _T("Creation Failed"), _T("reset_button_handle_"), MB_OK);
			DestroyWindow(window_handle_);
			return FALSE;
		}

		if (
			run_button_handle_ = CreateWindow(
			    _T("BUTTON")
			  , run_button_text_
			  , WS_TABSTOP | WS_CHILD | WS_VISIBLE | BS_PUSHBUTTON | BS_ICON
			  , height_0 << 1
			  , y_offset_1
			  , height_0
			  , height_0
			  , window_handle_
			  , reinterpret_cast<HMENU>(IDM_PATHPLANNER_RUN)
			  , application_handle
			  , 0
			)
		)
		{
			pause_icon_handle_ = LoadIcon(application_handle, MAKEINTRESOURCE(IDI_PAUSE));
			play_icon_handle_ = LoadIcon(application_handle, MAKEINTRESOURCE(IDI_PLAY));
			SendMessage(run_button_handle_, BM_SETIMAGE, IMAGE_ICON,
			            reinterpret_cast<LPARAM>(play_icon_handle_));
			EnableWindow(run_button_handle_, current_planner_->shouldEnableRun());
		}
		else
		{
			MessageBox(window_handle_, _T("Creation Failed"), _T("run_button_handle_"), MB_OK);
			DestroyWindow(window_handle_);
			return FALSE;
		}

		if (
			step_button_handle_ = CreateWindow(
			    _T("BUTTON")
			  , step_button_text_
			  , WS_TABSTOP | WS_CHILD | WS_VISIBLE | BS_PUSHBUTTON | BS_ICON
			  , height_0 * 3
			  , y_offset_1
			  , height_0
			  , height_0
			  , window_handle_
			  , reinterpret_cast<HMENU>(IDM_PATHPLANNER_STEP)
			  , application_handle
			  , 0
			)
		)
		{
			icon_handle = LoadIcon(application_handle, MAKEINTRESOURCE(IDI_PLAY_PAUSE));
			SendMessage(step_button_handle_, BM_SETIMAGE, IMAGE_ICON,
			            reinterpret_cast<LPARAM>(icon_handle));
			EnableWindow(step_button_handle_, current_planner_->shouldEnableRun());
		}
		else
		{
			MessageBox(window_handle_, _T("Creation Failed"), _T("step_button_handle_"), MB_OK);
			DestroyWindow(window_handle_);
			return FALSE;
		}

		if (
			time_run_button_handle_ = CreateWindow(
			    _T("BUTTON")
			  , time_run_button_text_
			  , WS_TABSTOP | WS_CHILD | WS_VISIBLE | BS_PUSHBUTTON | BS_ICON
			  , height_0 << 2
			  , y_offset_1
			  , height_0
			  , height_0
			  , window_handle_
			  , reinterpret_cast<HMENU>(IDM_PATHPLANNER_TIME_RUN)
			  , application_handle
			  , 0
			)
		)
		{
			icon_handle = LoadIcon(application_handle, MAKEINTRESOURCE(IDI_PLAY_END));
			SendMessage(time_run_button_handle_, BM_SETIMAGE, IMAGE_ICON,
			            reinterpret_cast<LPARAM>(icon_handle));
			EnableWindow(time_run_button_handle_, current_planner_->shouldEnableTimeRun());
		}
		else
		{
			MessageBox(window_handle_, _T("Creation Failed"), _T("time_run_button_handle_"),
			           MB_OK);
			DestroyWindow(window_handle_);
			return FALSE;
		}

		LONG const x_offset = 812;
		LONG const y_offset_2 = 24;
		LONG const width = client_rectangle.right - 815;
		LONG const height_1 = 48;

		LoadString(application_handle, IDS_LV_GLOBALS, buffer, MAX_LOADSTRING);

		if (
			globals_list_view_handle_ = CreateWindow(
			    WC_LISTVIEW
			  , buffer
			  , WS_CHILD | WS_VISIBLE | LVS_REPORT | LVS_EDITLABELS
			  , x_offset
			  , y_offset_2 << 1
			  , width
			  , height_1
			  , window_handle_
			  , 0
			  , application_handle
			  , 0
			)
		)
		{
			EnableWindow(globals_list_view_handle_, FALSE);
		}
		else
		{
			MessageBox(window_handle_, _T("Creation Failed"), _T("globals_list_view_handle_"),
			           MB_OK);
			DestroyWindow(window_handle_);
			return FALSE;
		}

		LONG const y_offset_4 = (y_offset_2 << 1) + height_1;
		LONG const height_2 = 144;

		LoadString(application_handle, IDS_LV_PARAMETERS, buffer, MAX_LOADSTRING);

		if (!(
			parameter_list_view_handle_ = CreateWindow(
			    WC_LISTVIEW
			  , buffer
			  , WS_CHILD | WS_VISIBLE | LVS_REPORT | LVS_EDITLABELS
			  , x_offset
			  , y_offset_4
			  , width
			  , height_2
			  , window_handle_
			  , 0
			  , application_handle
			  , 0
			)
		))
		{
			MessageBox(window_handle_, _T("Creation Failed"), _T("parameter_list_view_handle_"),
			           MB_OK);
			DestroyWindow(window_handle_);
			return FALSE;
		}

		window_class_ex.lpfnWndProc = tileGridProcedure;
		LoadString(application_handle, IDC_TILEGRID, app_class_string, MAX_LOADSTRING);
		RegisterClassEx(&window_class_ex);

		if (!(
			tile_grid_handle_ = CreateWindow(
			    app_class_string
			  , 0
			  , WS_CHILD | WS_VISIBLE
			  , tile_grid_x_
			  , tile_grid_y_
			  , tile_grid_width_
			  , tile_grid_height_
			  , window_handle_
			  , 0
			  , application_handle
			  , 0
			)
		))
		{
			MessageBox(window_handle_, _T("Creation Failed"), _T("tile_grid_handle_"), MB_OK);
			DestroyWindow(window_handle_);
			return FALSE;
		}
/*
		if (icon_handle)
		{
			HWND tooltip_handle;
			TOOLINFO tool_info = { 0 };

			tool_info.cbSize = sizeof(tool_info);
			tool_info.hwnd = window_handle_;
			tool_info.hinst = application_handle;
			tool_info.uFlags = TTF_IDISHWND | TTF_SUBCLASS | TTF_TRANSPARENT;

			if (
				tooltip_handle = CreateWindowEx(
				    WS_EX_TOPMOST
				  , TOOLTIPS_CLASS
				  , 0
				  , (
				        WS_POPUP
				      | TTS_NOPREFIX
				      | TTS_ALWAYSTIP
				      | TTS_BALLOON
				    )
				  , CW_USEDEFAULT
				  , CW_USEDEFAULT
				  , CW_USEDEFAULT
				  , CW_USEDEFAULT
				  , window_handle_
				  , 0
				  , application_handle
				  , 0
				)
			)
			{
				SetWindowPos(
				    tooltip_handle
				  , HWND_TOPMOST
				  , 0
				  , 0
				  , 0
				  , 0
				  , SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE
				);
//				tool_info.uId = GetWindowLong(open_button_handle_, GWL_ID)
				tool_info.uId = reinterpret_cast<UINT_PTR>(
					open_button_handle_
				);
				tool_info.lpszText = open_button_text_;
				SendMessage(tooltip_handle, TTM_ADDTOOL, 0, reinterpret_cast<LPARAM>(&tool_info));
				SendMessage(tooltip_handle, TTM_ACTIVATE, TRUE, 0);
			}
		}
*/

		LVCOLUMN lv_column;

		lv_column.mask = LVCF_FMT | LVCF_WIDTH | LVCF_TEXT | LVCF_SUBITEM;
		lv_column.pszText = buffer;
		lv_column.fmt = LVCFMT_RIGHT;
		lv_column.cx = 94;
		LoadString(application_handle, IDS_LVC_VALUE, buffer, MAX_LOADSTRING);
		ListView_InsertColumn(globals_list_view_handle_, lv_column.iSubItem = 0, &lv_column);
		ListView_InsertColumn(parameter_list_view_handle_, lv_column.iSubItem, &lv_column);
		lv_column.fmt = LVCFMT_LEFT;
		lv_column.cx = 155;
		LoadString(application_handle, IDS_LVC_GLOBAL_SETTING, buffer, MAX_LOADSTRING);
		ListView_InsertColumn(globals_list_view_handle_, lv_column.iSubItem = 1, &lv_column);
		LoadString(application_handle, IDS_LVC_PARAM_NAME, buffer, MAX_LOADSTRING);
		ListView_InsertColumn(parameter_list_view_handle_, lv_column.iSubItem, &lv_column);

		LVITEM list_view_item;

		list_view_item.mask = LVIF_TEXT;
		list_view_item.pszText = LPSTR_TEXTCALLBACK;
		list_view_item.iItem = 0;
		list_view_item.iSubItem = 0;
		ListView_InsertItem(globals_list_view_handle_, &list_view_item);
		list_view_item.iSubItem = 1;
		SendMessage(globals_list_view_handle_, LVM_SETITEM, 0,
		            reinterpret_cast<LPARAM>(&list_view_item));

		tile_grid_device_context_handle_ = GetDC(tile_grid_handle_);
		tile_grid_buffer_context_handle_ = CreateCompatibleDC(tile_grid_device_context_handle_);
		tile_grid_buffer_bitmap_handle_ = CreateCompatibleBitmap(tile_grid_device_context_handle_,
		                                                         tile_grid_width_,
		                                                         tile_grid_height_);
		_stprintf(buffer, _T("%s"), _T(USEDEFAULTMAP));
		openFile_(window_handle_, buffer);
		ShowWindow(window_handle_, n_cmd_show);
		ResumeThread(thread_handle_);
		UpdateWindow(window_handle_);

		return TRUE;
	}
	else
	{
		return FALSE;
	}
}

void PathPlannerLab::renderFull_()
{
	BitBlt(
	    tile_grid_buffer_context_handle_
	  , 0
	  , 0
	  , tile_grid_width_
	  , tile_grid_height_
	  , tile_grid_buffer_context_handle_
	  , 0
	  , 0
	  , BLACKNESS
	);
	paintAllTileGrid_(tile_grid_device_context_handle_, tile_grid_buffer_context_handle_);
	needs_full_render_ = current_planner_->needsFullRedraw();
}

void PathPlannerLab::threadHandler()
{
	HGDIOBJ old_tile_grid_bitmap_handle = SelectObject(tile_grid_buffer_context_handle_,
	                                                   tile_grid_buffer_bitmap_handle_);

	for (;;)
	{
		thread_sync_.acquire();

		if (should_stop_thread_handle_)
		{
			SelectObject(tile_grid_buffer_context_handle_, old_tile_grid_bitmap_handle);
			cannot_close_thread_handle_ = false;
			thread_sync_.release();
			return;
		}

		thread_sync_.release();
		planner_sync_.acquire();

		if (PathPlannerGlobals::getInstance()->isFlagOn(PathPlannerGlobals::SHOW_SEARCH_RUNNING))
		{
			if (current_planner_->isRunnableByKey())
			{
				if (needs_full_render_)
				{
					current_planner_->runSearch(/*10*/);//run search for a certain timestep
					renderFull_();
				}
				else
				{
					current_planner_->beginRedrawSearchProgress(tile_grid_offset_,
					                                            tile_grid_width_,
					                                            tile_grid_height_,
					                                            tile_grid_buffer_context_handle_);
					current_planner_->runSearch(/*10*/);
					current_planner_->endRedrawSearchProgress(tile_grid_offset_,
					                                          tile_grid_width_,
					                                          tile_grid_height_,
					                                          tile_grid_buffer_context_handle_);
					doubleBufferTileGrid_(tile_grid_device_context_handle_,
					                      tile_grid_buffer_context_handle_);
				}

				planner_sync_.release();
			}
			else if (current_planner_->isInitializableByKey())
			{
				current_planner_->setTimedSearch(false);
				current_planner_->initializeSearch();
				current_planner_->checkSolution(window_handle_);
				renderFull_();
				planner_sync_.release();
			}
			else
			{
				PathPlannerGlobals::getInstance()->turnOff(PathPlannerGlobals::SHOW_SEARCH_RUNNING);
				showRunStopped_();
				updateRunButtons_();
				current_planner_->checkSolution(window_handle_);
				planner_sync_.release();
				InvalidateRect(tile_grid_handle_, 0, FALSE);
			}
		}
		else
		{
			planner_sync_.release();
		}

		Sleep(25);
	}
}

void PathPlannerLab::reset_()
{
	PathPlannerGlobals::getInstance()->turnOff(PathPlannerGlobals::SHOW_SEARCH_RUNNING);
	current_planner_->resetSearch();
	showRunStopped_();
	updateRunButtons_();
}

void PathPlannerLab::onReset()
{
	planner_sync_.acquire();
	reset_();
	planner_sync_.release();
	InvalidateRect(tile_grid_handle_, 0, FALSE);
}

void PathPlannerLab::resetByKey_()
{
	planner_sync_.acquire();

	if (current_planner_->isReady())
	{
		reset_();
		planner_sync_.release();
		InvalidateRect(tile_grid_handle_, 0, FALSE);
	}
	else
	{
		planner_sync_.release();
	}
}

void PathPlannerLab::onSetStart()
{
	planner_sync_.acquire();

	if (current_planner_->updateStart())
	{
		planner_sync_.release();
		InvalidateRect(tile_grid_handle_, 0, FALSE);
	}
	else
	{
		planner_sync_.release();
	}
}

void PathPlannerLab::onSetGoal()
{
	planner_sync_.acquire();

	if (current_planner_->updateGoal())
	{
		planner_sync_.release();
		InvalidateRect(tile_grid_handle_, 0, FALSE);
	}
	else
	{
		planner_sync_.release();
	}
}

bool PathPlannerLab::run_()
{
	if (PathPlannerGlobals::getInstance()->isFlagOn(PathPlannerGlobals::SHOW_SEARCH_RUNNING))
	{
		PathPlannerGlobals::getInstance()->turnOff(PathPlannerGlobals::SHOW_SEARCH_RUNNING);
		showRunStopped_();
		updateRunButtons_();
		current_planner_->checkSolution(window_handle_);
		return true;
	}
	else
	{
		BitBlt(
		    tile_grid_buffer_context_handle_
		  , 0
		  , 0
		  , tile_grid_width_
		  , tile_grid_height_
		  , tile_grid_buffer_context_handle_
		  , 0
		  , 0
		  , BLACKNESS
		);
		PathPlannerGlobals::getInstance()->turnOn(PathPlannerGlobals::SHOW_SEARCH_RUNNING);
		paintAllTileGrid_(
		    tile_grid_device_context_handle_
		  , tile_grid_buffer_context_handle_
		);
		return false;
	}
}

void PathPlannerLab::onRun()
{
	planner_sync_.acquire();

	if (run_())
	{
		planner_sync_.release();
		InvalidateRect(tile_grid_handle_, 0, FALSE);
	}
	else
	{
		planner_sync_.release();
		SendMessage(
		    run_button_handle_
		  , BM_SETIMAGE
		  , IMAGE_ICON
		  , reinterpret_cast<LPARAM>(pause_icon_handle_)
		);
	}
}

void PathPlannerLab::runByKey_()
{
	planner_sync_.acquire();

	if (current_planner_->isReady())
	{
		if (current_planner_->isRunnableByKey() || current_planner_->isInitializableByKey())
		{
			if (run_())
			{
				planner_sync_.release();
				InvalidateRect(tile_grid_handle_, 0, FALSE);
			}
			else
			{
				planner_sync_.release();
				SendMessage(
				    run_button_handle_
				  , BM_SETIMAGE
				  , IMAGE_ICON
				  , reinterpret_cast<LPARAM>(pause_icon_handle_)
				);
			}

			return;
		}
	}

	planner_sync_.release();
}

void PathPlannerLab::step_()
{
	PathPlannerGlobals::getInstance()->turnOff(PathPlannerGlobals::SHOW_SEARCH_RUNNING);

	if (current_planner_->isInitializableByKey())
	{
		current_planner_->setTimedSearch(false);
		//current_planner_->resetSearch();
		current_planner_->initializeSearch();
		current_planner_->stepSearch(/*0*/);//one step at a time
	}
	else if (current_planner_->isRunnableByKey())
	{
		current_planner_->stepSearch(/*0*/);//one step at a time
	}

	showRunStopped_();
	updateRunButtons_();
	current_planner_->checkSolution(window_handle_);
}

void PathPlannerLab::onStep()
{
	planner_sync_.acquire();
	step_();
	planner_sync_.release();
	InvalidateRect(tile_grid_handle_, 0, FALSE);
}

void PathPlannerLab::stepByKey_()
{
	planner_sync_.acquire();

	if (current_planner_->isReady())
	{
		step_();
		planner_sync_.release();
		InvalidateRect(tile_grid_handle_, 0, FALSE);
	}
	else
	{
		planner_sync_.release();
	}
}

void PathPlannerLab::timeRun_()
{
	PathPlannerGlobals::getInstance()->turnOff(PathPlannerGlobals::SHOW_SEARCH_RUNNING);
	showRunStopped_();
	EnableWindow(start_waypoint_button_handle_, FALSE);
	EnableWindow(goal_waypoint_button_handle_, FALSE);
	EnableWindow(run_button_handle_, FALSE);
	EnableWindow(step_button_handle_, FALSE);
	EnableWindow(time_run_button_handle_, FALSE);
	current_planner_->resetSearch();
	current_planner_->timeSearch();
	updateButtons_();
	//current_planner_->checkSolution(window_handle_);//we can't test the solution on the timed run
}

void PathPlannerLab::onTimeRun()
{
	planner_sync_.acquire();
	timeRun_();
	planner_sync_.release();
	InvalidateRect(tile_grid_handle_, 0, FALSE);
}

void PathPlannerLab::timeRunByKey_()
{
	planner_sync_.acquire();

	if (current_planner_->isReady())
	{
		timeRun_();
		planner_sync_.release();
		InvalidateRect(tile_grid_handle_, 0, FALSE);
	}
	else
	{
		planner_sync_.release();
	}
}

bool PathPlannerLab::onKeyPress(WPARAM w_param)
{
	if (tile_map_width_ && tile_map_height_)
	{
		switch (w_param)
		{
			case VK_BACK:
			{
				resetByKey_();
				return true;
			}

			case VK_SPACE:
			{
				runByKey_();
				return true;
			}

			case VK_ADD:
			{
				stepByKey_();
				return true;
			}

			case VK_TAB:
			{
				timeRunByKey_();
				return true;
			}

			case 0x53:  // 'S'
			{
				onSetStart();
				return true;
			}

			case 0x47:  // 'G'
			{
				onSetGoal();
				return true;
			}
		}
	}

	return false;
}

LRESULT PathPlannerLab::onListView(LPARAM l_param)
{
	NMHDR* p_nmhdr = reinterpret_cast<NMHDR*>(l_param);

	switch (p_nmhdr->code)
	{
		case LVN_KEYDOWN:
		{
			onKeyPress(reinterpret_cast<NMLVKEYDOWN*>(l_param)->wVKey);
			break;
		}

		case NM_CLICK:
		{
			NMITEMACTIVATE* p_nm_item = reinterpret_cast<NMITEMACTIVATE*>(l_param);
			HWND list_view_handle = p_nmhdr->hwndFrom;
			int row = p_nm_item->iItem;

			// Ensure that a list-view item was selected.
			if (-1 != row)
			{
				// Don't take any chances with editable list-views.
				planner_sync_.acquire();
				PathPlannerGlobals::getInstance()->turnOff(
					PathPlannerGlobals::SHOW_SEARCH_RUNNING
				);
				showRunStopped_();
				updateRunButtons_();
				planner_sync_.release();
				ListView_EditLabel(list_view_handle, row);
			}
		}

		case LVN_ITEMACTIVATE:
		{
			NMITEMACTIVATE* p_nm_item = reinterpret_cast<NMITEMACTIVATE*>(l_param);
			int row = p_nm_item->iItem;

			// Ensure that a list-view item was selected.
			if (-1 != row)
			{
				HWND list_view_handle = p_nmhdr->hwndFrom;

				// Don't take any chances with editable list-views.
				planner_sync_.acquire();
				PathPlannerGlobals::getInstance()->turnOff(
					PathPlannerGlobals::SHOW_SEARCH_RUNNING
				);
				showRunStopped_();
				updateRunButtons_();
				planner_sync_.release();
				ListView_EditLabel(list_view_handle, row);
			}

			break;
		}

		case LVN_ENDLABELEDIT:
		{
			NMLVDISPINFO* list_view_display_info = reinterpret_cast<NMLVDISPINFO*>(l_param);
			LVITEM& item = list_view_display_info->item;

			// Ensure that the user has not cancelled label editing.
			if (item.pszText)
			{
				if (p_nmhdr->hwndFrom == parameter_list_view_handle_)
				{
					planner_sync_.acquire();

					if (current_planner_->updateInput(list_view_display_info))
					{
						planner_sync_.release();
						InvalidateRect(tile_grid_handle_, 0, FALSE);
						return TRUE;
					}
					else
					{
						planner_sync_.release();
					}
				}
				else// if (p_nmhdr->hwndFrom == globals_list_view_handle_)
				{
					switch (item.iItem)
					{
						case 0:
						{
							double radius = _tstof(item.pszText);

							if (radius < 4.0)
							{
								radius = 4.0;
							}

							PathPlannerGlobals::getInstance()->setMinTileRadius(radius);
							return TRUE;
						}
					}
				}
			}

			return FALSE;
		}

		case NM_CUSTOMDRAW:
		{
			return CDRF_DODEFAULT;
		}

		case LVN_GETDISPINFO:
		{
			NMLVDISPINFO* list_view_display_info = reinterpret_cast<NMLVDISPINFO*>(l_param);
			HWND list_view_handle = p_nmhdr->hwndFrom;

			if (list_view_handle == parameter_list_view_handle_)
			{
				planner_sync_.acquire();
				current_planner_->displayInput(list_view_display_info);
				planner_sync_.release();
			}
			else// if (list_view_handle == globals_list_view_handle_)
			{
				LVITEM& item = list_view_display_info->item;

				switch (item.iItem)
				{
					case 0:
					{
						if (item.iSubItem)
						{
							_stprintf(
							    item.pszText
							  , _T("%s")
							  , _T("Minimum Tile Radius")
							);
						}
						else
						{
							_stprintf(
							    item.pszText
							  , _T("%1.6f")
							  , PathPlannerGlobals::getInstance()->getMinTileRadius()
							);
						}

						break;
					}
				}
			}

			break;
		}
	}

	return FALSE;
}

void PathPlannerLab::onSize(HWND window_handle, WPARAM w_param, LPARAM l_param)
{
	SCROLLINFO horizontal_scroll_info;
	SCROLLINFO vertical_scroll_info;

	// Get the client dimensions and set the scrollbar properties.
	horizontal_scroll_info.cbSize = vertical_scroll_info.cbSize = sizeof(SCROLLINFO);
	horizontal_scroll_info.fMask = vertical_scroll_info.fMask = (
	    SIF_RANGE
	  | SIF_PAGE
	  | SIF_DISABLENOSCROLL
	);
	horizontal_scroll_info.nMin = vertical_scroll_info.nMin = 0;
	horizontal_scroll_info.nPage = LOWORD(l_param);
	vertical_scroll_info.nPage = HIWORD(l_param);
	horizontal_scroll_info.nMax = tile_map_width_ - 1;
	vertical_scroll_info.nMax = tile_map_height_ - 1;

	// Enable the horizontal scroll bar.
	SetScrollInfo(window_handle, SB_HORZ, &horizontal_scroll_info, TRUE);

	// Enable the vertical scroll bar.
	SetScrollInfo(window_handle, SB_VERT, &vertical_scroll_info, TRUE);
}

void PathPlannerLab::onHScroll(HWND window_handle, WPARAM w_param, LPARAM l_param)
{
	SCROLLINFO horizontal_scroll_info;

	// Get all information pertaining to the horizontal scroll bar.
	horizontal_scroll_info.cbSize = sizeof(SCROLLINFO);
	horizontal_scroll_info.fMask  = SIF_ALL;
	GetScrollInfo(window_handle, SB_HORZ, &horizontal_scroll_info);

	// Save the old position for later comparison.
	int const old_position = horizontal_scroll_info.nPos;

	// Adjust the scroll position based upon the scroll request.
	switch (LOWORD(w_param))
	{
		case SB_LEFT:
			horizontal_scroll_info.nPos = horizontal_scroll_info.nMin;
			break;
		case SB_RIGHT:
			horizontal_scroll_info.nPos = horizontal_scroll_info.nMax;
			break;
		case SB_LINELEFT:
			horizontal_scroll_info.nPos -= 1;
			break;
		case SB_LINERIGHT:
			horizontal_scroll_info.nPos += 1;
			break;
		case SB_PAGELEFT:
			horizontal_scroll_info.nPos -= horizontal_scroll_info.nPage;
			break;
		case SB_PAGERIGHT:
			horizontal_scroll_info.nPos += horizontal_scroll_info.nPage;
			break;
		case SB_THUMBTRACK:
			horizontal_scroll_info.nPos = horizontal_scroll_info.nTrackPos;
			break;
	}

	// Set the new, horizontal scroll position.
	horizontal_scroll_info.fMask = SIF_POS;
	SetScrollInfo(window_handle, SB_HORZ, &horizontal_scroll_info, TRUE);
	GetScrollInfo(window_handle, SB_HORZ, &horizontal_scroll_info);

	if (old_position != horizontal_scroll_info.nPos)
	{
		planner_sync_.acquire();
		tile_grid_offset_.x = -horizontal_scroll_info.nPos;

		if (PathPlannerGlobals::getInstance()->isFlagOn(PathPlannerGlobals::SHOW_SEARCH_RUNNING))
		{
			needs_full_render_ = true;
			planner_sync_.release();
		}
		else
		{
			planner_sync_.release();
			InvalidateRect(tile_grid_handle_, 0, FALSE);
		}
	}
}

void PathPlannerLab::onVScroll(HWND window_handle, WPARAM w_param, LPARAM l_param)
{
	SCROLLINFO vertical_scroll_info;

	// Get all information pertaining to the vertical scroll bar.
	vertical_scroll_info.cbSize = sizeof(SCROLLINFO);
	vertical_scroll_info.fMask  = SIF_ALL;
	GetScrollInfo(window_handle, SB_VERT, &vertical_scroll_info);

	// Save the old position for later comparison.
	int const old_position = vertical_scroll_info.nPos;

	// Adjust the scroll position based upon the scroll request.
	switch (LOWORD(w_param))
	{
		case SB_LEFT:
			vertical_scroll_info.nPos = vertical_scroll_info.nMin;
			break;
		case SB_RIGHT:
			vertical_scroll_info.nPos = vertical_scroll_info.nMax;
			break;
		case SB_LINELEFT:
			vertical_scroll_info.nPos -= 1;
			break;
		case SB_LINERIGHT:
			vertical_scroll_info.nPos += 1;
			break;
		case SB_PAGELEFT:
			vertical_scroll_info.nPos -= vertical_scroll_info.nPage;
			break;
		case SB_PAGERIGHT:
			vertical_scroll_info.nPos += vertical_scroll_info.nPage;
			break;
		case SB_THUMBTRACK:
			vertical_scroll_info.nPos = vertical_scroll_info.nTrackPos;
			break;
	}

	// Set the new, vertical scroll position.
	vertical_scroll_info.fMask = SIF_POS;
	SetScrollInfo(window_handle, SB_VERT, &vertical_scroll_info, TRUE);
	GetScrollInfo(window_handle, SB_VERT, &vertical_scroll_info);

	if (old_position != vertical_scroll_info.nPos)
	{
		planner_sync_.acquire();
		tile_grid_offset_.y = -vertical_scroll_info.nPos;

		if (PathPlannerGlobals::getInstance()->isFlagOn(PathPlannerGlobals::SHOW_SEARCH_RUNNING))
		{
			needs_full_render_ = true;
			planner_sync_.release();
		}
		else
		{
			planner_sync_.release();
			InvalidateRect(tile_grid_handle_, 0, FALSE);
		}
	}
}

void PathPlannerLab::onFileOpen(HWND window_handle)
{
	OPENFILENAME open_file_name;
	TCHAR        file_name[MAX_BUFFER_SIZE];
	TCHAR        initial_directory[MAX_BUFFER_SIZE];

	ZeroMemory(&open_file_name, sizeof(open_file_name));
	open_file_name.lStructSize = sizeof(open_file_name);
	open_file_name.hwndOwner = window_handle;
	open_file_name.lpstrFile = file_name;

	// Set lpstrFile[0] to '\0' so that GetOpenFileName does not 
	// use the contents of file_name to initialize itself.
	open_file_name.lpstrFile[0] = '\0';
	open_file_name.nMaxFile = sizeof(file_name);
	open_file_name.lpstrFilter = text_filter_;
	open_file_name.nFilterIndex = 1;
	open_file_name.lpstrFileTitle = 0;
	open_file_name.nMaxFileTitle = 0;
	open_file_name.lpstrInitialDir = initial_directory;
	_stprintf(initial_directory, _T("%s"), _T("Data"));
	open_file_name.Flags = OFN_PATHMUSTEXIST | OFN_FILEMUSTEXIST;

	if (GetOpenFileName(&open_file_name))
	{
		openFile_(window_handle, file_name);
	}
}

void PathPlannerLab::doubleBufferTileGrid_(HDC device_context_handle,
                                           HDC buffer_context_handle) const
{
	BitBlt(
	    device_context_handle
	  , 0
	  , 0
	  , tile_grid_width_
	  , tile_grid_height_
	  , buffer_context_handle
	  , 0
	  , 0
	  , SRCCOPY
	);
}

void PathPlannerLab::paintAllTileGrid_(HDC device_context_handle, HDC buffer_context_handle) const
{
	drawGrid(
	    ground_up_tile_map_
	  , tile_grid_offset_
	  , tile_grid_width_
	  , tile_grid_height_
	  , buffer_context_handle
	);
	current_planner_->displaySearchProgress(
	    tile_grid_offset_
	  , tile_grid_width_
	  , tile_grid_height_
	  , buffer_context_handle
	);
	doubleBufferTileGrid_(device_context_handle, buffer_context_handle);
}

void PathPlannerLab::paintTileGrid(HDC device_context_handle) const
{
	HDC buffer_context_handle = CreateCompatibleDC(device_context_handle);
	HBITMAP buffer_bitmap_handle = CreateCompatibleBitmap(
	    device_context_handle
	  , tile_grid_width_
	  , tile_grid_height_
	);
	HGDIOBJ old_bitmap_handle = SelectObject(
	    buffer_context_handle
	  , buffer_bitmap_handle
	);

	planner_sync_.acquire();
	paintAllTileGrid_(device_context_handle, buffer_context_handle);
	planner_sync_.release();
	SelectObject(buffer_context_handle, old_bitmap_handle);
	DeleteObject(buffer_bitmap_handle);
	DeleteDC(buffer_context_handle);
}



void BindStdHandlesToConsole()
{
	// Redirect the CRT standard input, output, and error handles to the console
	FILE *result = freopen("CONIN$", "r", stdin);
	result = freopen("CONOUT$", "w", stdout);
	result = freopen("CONOUT$", "w", stderr);

	//Clear the error state for each of the C++ standard stream objects. We need to do this, as
	//attempts to access the standard streams before they refer to a valid target will cause the
	//iostream objects to enter an error state. In versions of Visual Studio after 2005, this seems
	//to always occur during startup regardless of whether anything has been read from or written to
	//the console or not.
	std::wcout.clear();
	std::cout.clear();
	std::wcerr.clear();
	std::cerr.clear();
	std::wcin.clear();
	std::cin.clear();
}

int APIENTRY _tWinMain(HINSTANCE application_handle,
                       HINSTANCE previous_application_handle,
                       LPTSTR    command_line,
                       int       n_cmd_show)
{
	UNREFERENCED_PARAMETER(previous_application_handle);
	UNREFERENCED_PARAMETER(command_line);

//#ifdef _DEBUG
	// Set up the console window.
	CONSOLE_SCREEN_BUFFER_INFO coninfo;

	AllocConsole();
	GetConsoleScreenBufferInfo(GetStdHandle(STD_OUTPUT_HANDLE), &coninfo);
	coninfo.dwSize.Y = 500;
	SetConsoleScreenBufferSize(GetStdHandle(STD_OUTPUT_HANDLE), coninfo.dwSize);

	//// Redirect standard output to the console window.
	//HANDLE standard_handle = GetStdHandle(STD_OUTPUT_HANDLE);
	//int console_handle = _open_osfhandle(reinterpret_cast<intptr_t>(standard_handle), _O_TEXT);

	//*stdout = *_fdopen(console_handle, "w");
	//setvbuf(stdout, 0, _IONBF, 0);

	//// Redirect standard error to the console window.
	//standard_handle = GetStdHandle(STD_ERROR_HANDLE);
	//console_handle = _open_osfhandle(reinterpret_cast<intptr_t>(standard_handle), _O_TEXT);
	//*stderr = *_fdopen(console_handle, "w");
	//setvbuf(stderr, 0, _IONBF, 0);

	//// Allow C++ code to benefit from console redirection.
	//ios::sync_with_stdio(false);


	BindStdHandlesToConsole();

//#endif

	// Perform application initialization:
	if (!PathPlannerLab::getInstance()->initializeApplication(application_handle, n_cmd_show))
	{
		PathPlannerLab::deleteInstance();
		PathPlannerGlobals::deleteInstance();
		return FALSE;
	}

	MSG msg;
	HACCEL accelerator_table_handle = LoadAccelerators(
	    application_handle
	  , MAKEINTRESOURCE(IDC_PATHPLANNER)
	);

	// Main message loop:
	while (GetMessage(&msg, 0, 0, 0))
	{
		if (!TranslateAccelerator(msg.hwnd, accelerator_table_handle, &msg))
		{
			TranslateMessage(&msg);
			DispatchMessage(&msg);
		}
	}

	PathPlannerLab::deleteInstance();
	PathPlannerGlobals::deleteInstance();
	return static_cast<int>(msg.wParam);
}

long long cpu_clock()
{
	static FILETIME ctime;
	static FILETIME etime;
	static FILETIME ktime;
	static FILETIME utime;
	static HANDLE handle;

	handle = GetCurrentProcess();
	GetProcessTimes(GetCurrentProcess(), &ctime, &etime, &ktime, &utime);
	return ((long long)(utime.dwHighDateTime + ktime.dwHighDateTime) << 32) + utime.dwLowDateTime + ktime.dwLowDateTime;
}