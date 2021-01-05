// Code by Cromwell D. Enage, Jeremiah Blanchard
// August 2010
#pragma once

#include <istream>
#include "../TileSystem/TileMap.h"

//! \brief Loads a tile map from the specified input stream.
template <typename CharT, typename CharTraits>
bool load(std::basic_istream<CharT,CharTraits>& input_stream, ufl_cap4053::TileMap& tile_map)
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

		tile_map.computeWeightSumSquared();
		return true;
	}

	return false;
}

