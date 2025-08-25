// Code by Cromwell D. Enage, Jeremiah Blanchard
// August 2010
// Updated February 2021, Jeremiah Blanchard
#pragma once

#include <istream>
#include "TileMap.h"

//! \brief Loads a tile map from the specified input stream.
template <typename CharT, typename CharTraits>
bool loadTileMapFromStream(std::basic_istream<CharT,CharTraits>& input_stream, ufl_cap4053::TileMap& tile_map)
{
	tile_map.reset();

	int row_count = 0;
	int column_count = 0;

	if (!input_stream.eof() && (input_stream >> row_count) && (input_stream >> column_count))
	{
		tile_map.createTileArray(row_count, column_count);

		int row;
		int column;
		unsigned int data;

		for (row = 0; row < row_count; ++row)
		{
			for (column = 0; column < column_count; ++column)
			{
				if (input_stream >> data)
				{
					// Bad Things Will Happen(tm) if we don't cast here.
					tile_map.addTile(row, column, static_cast<unsigned char>(data));
				}
				else
				{
					tile_map.reset();
					return false;
				}
			}
		}

		int start_row;
		int start_column;

		if (!input_stream.eof() && (input_stream >> start_row) && (input_stream >> start_column))
		{
			tile_map.setStartTile(start_row, start_column);
		}

		int goal_row;
		int goal_column;

		if (!input_stream.eof() && (input_stream >> goal_row) && (input_stream >> goal_column))
		{
			tile_map.setGoalTile(goal_row, goal_column);
		}

		tile_map.computeWeightSumSquared();
		return true;
	}

	return false;
}

