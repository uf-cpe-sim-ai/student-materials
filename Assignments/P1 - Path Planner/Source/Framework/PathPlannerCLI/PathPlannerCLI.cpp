// Path Planner CLI.
// Code by Jeremiah Blanchard, February 2021

#include <cmath>
#include <fstream>
#include <iostream>

// Some standard data structures
#include <limits>
#include <sstream>
#include <string>
#include <vector>
#include <exception>

// Path planner specific
#include "../TileSystem/TileMap.h"
#include "../TileSystem/TileMapUtility.h"
#include "../../PathSearch/PathSearch.h"

using namespace ufl_cap4053;
using namespace ufl_cap4053::searches;

#define DEFAULT_RADIUS 6
#define DEFAULT_REPETITIONS 1

// Forward declarations
std::string caseTest(int startR, int startC, int goalR, int goalC, TileMap& tileMap, int repetitions);

// Determine the test type and run it.
int main(int argc, char* argv[])
{
	TileMap tileMap;
	int repetitions = DEFAULT_REPETITIONS;

	if (argc < (2))
	{
		std::cout << "Usage: " << argv[0] << " MAPFILE [START_ROW START_COL GOAL_ROW GOAL_COL] [REPETITIONS] [TILE_RADIUS]\n\n";
		return EXIT_SUCCESS;
	}

	try
	{
		// Get the file name for the map file
		char* filename = argv[1];

		// Load the tilemap, and terminate if an invalid filename is provided.
		std::ifstream fileIn(filename);
		if (fileIn.is_open() == false)
		{
			std::cerr << "Could not open map file.\n";
			return false;
		}
		else if (!loadTileMapFromStream(fileIn, tileMap))
		{
			std::cerr << "Could not load map file.\n";
			fileIn.close();
			return false;
		}

		// Set up the start and goal tile coordinates
		int startRow, startCol, goalRow, goalCol;
		if (argc > 5)
		{
			startRow = atoi(argv[2]);
			startCol = atoi(argv[3]);
			goalRow = atoi(argv[4]);
			goalCol = atoi(argv[5]);
		}
		else
		{
			startRow = tileMap.getStartTile()->getRow();
			startCol = tileMap.getStartTile()->getColumn();
			goalRow = tileMap.getGoalTile()->getRow();
			goalCol = tileMap.getGoalTile()->getColumn();
		}

		// Grab the number of repetitions if it was provided
		if (argc > 6)
			repetitions = atoi(argv[6]);

		// Grab the tile radius if it was provided
		if (argc > 7)
			tileMap.setRadius(atoi(argv[7]));
		else
			tileMap.setRadius(DEFAULT_RADIUS);

		// Run the test and print the results.
		std::cout << "Running test on << " << filename << ": (" << startRow << "," << startCol << ")->(" << goalRow << "," << goalCol << "), " << repetitions << " time(s)... ";
		std::string result = caseTest(startRow, startCol, goalRow, goalCol, tileMap, repetitions);
		std::cout << "Done.\n";
		std::cout << result << "\n";

		return EXIT_SUCCESS;
	}
	catch (std::exception& e)
	{
		std::cout << "Caught exception: " << e.what() << "\n";
		return EXIT_FAILURE;
	}
	catch (std::string& e)
	{

		std::cout << "Caught exception: " << e << "\n";
		return EXIT_FAILURE;
	}
	catch (char* e)
	{
		std::cout << "Caught exception: " << e << "\n";
		return EXIT_FAILURE;
	}
	catch (int e)
	{

		std::cout << "Caught exception: " << e << "\n";
		return EXIT_FAILURE;
	}
	catch (...)
	{
		std::cout << "Caught unknown exception\n";
		return EXIT_FAILURE;
	}
}

// Run the specified test case; get the solution and measure leak measurement.
std::string caseTest(int startR, int startC, int goalR, int goalC, TileMap& tileMap, int repetitions)
{
	// Set up the output string stream
	std::ostringstream result;

	// Setup and run the student search.
	PathSearch search;
	search.unload();
	search.load(&tileMap);

	// Run the planner for the specified number of iterations (minus one - so we can grab the result).
	for (int repetition = 1; repetition < repetitions; repetition++)
	{
		search.initialize(startR, startC, goalR, goalC);
		search.update(LONG_MAX);
		search.shutdown();
	}

	// Run one last time, grab the solution, and return it as a string.
	search.initialize(startR, startC, goalR, goalC);
	search.update(LONG_MAX);
	std::vector<Tile const*> const& solution = search.getSolution();

	result << "[ ";
	for(auto tile : solution)
		result << "(" << tile->getRow() << "," << tile->getColumn() << ") ";

	result << "]";
	search.shutdown();
	search.unload();
	return result.str();
}