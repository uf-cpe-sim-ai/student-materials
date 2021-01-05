#include "TileMap.h"

namespace ufl_cap4053 {

	TileMap::TileMap()
		: row_count(0), column_count(0), tiles(0), tile_radius(0.0), weight_sum_squared(0)
	{
	}

	TileMap::TileMap(TileMap const& copy)
		: row_count(copy.row_count), column_count(copy.column_count)
		, tiles(new Tile*[row_count * column_count]), tile_radius(copy.tile_radius)
		, weight_sum_squared(copy.weight_sum_squared)
	{
		int n = row_count * column_count;

		while (n)
		{
			--n;
			tiles[n] = new Tile(*copy.tiles[n]);
		}
	}

	TileMap& TileMap::operator=(TileMap const& copy)
	{
		if (this != &copy)
		{
			if (tiles)
			{
				int n = row_count * column_count;

				while (n)
				{
					delete tiles[--n];
					tiles[n] = 0;
				}

				delete[] tiles;
			}

			row_count = copy.row_count;
			column_count = copy.row_count;
			tiles = new Tile*[row_count * column_count];
			tile_radius = copy.tile_radius;
			weight_sum_squared = copy.weight_sum_squared;

			int n = row_count * column_count;

			while (n)
			{
				--n;
				tiles[n] = new Tile(*copy.tiles[n]);
			}
		}

		return *this;
	}

	TileMap::~TileMap()
	{
		reset();
	}

	void TileMap::reset()
	{
		if (tiles)
		{
			int n = row_count * column_count;

			row_count = column_count = 0;
			tile_radius = 0.0;

			while (n)
			{
				delete tiles[--n];
				tiles[n] = 0;
			}

			delete[] tiles;
			tiles = 0;
		}

		weight_sum_squared = 0;
	}

	void TileMap::setRadius(double radius)
	{
		tile_radius = radius;

		if (tiles)
		{
			int n = row_count * column_count;

			while (n)
			{
				if (tiles[--n])
				{
					tiles[n]->setRadius(tile_radius);
				}
			}
		}
	}

	void TileMap::createTileArray(int num_rows, int num_columns)
	{
		reset();
		tiles = new Tile*[num_rows * num_columns];
		row_count = num_rows;
		column_count = num_columns;
	}

	void TileMap::addTile(int row, int column, unsigned char data)
	{
		tiles[row * column_count + column] = new Tile(row, column, tile_radius, data);
	}

	Tile* TileMap::getTile(int row, int column) const
	{
		if ((0 <= row) && (0 <= column) && (row < row_count) && (column < column_count))
		{
			return tiles[row * column_count + column];
		}
		else
		{
			// Maybe we should assert...naaah.
			return 0;
		}
	}

	void TileMap::computeWeightSumSquared()
	{
		unsigned int i = row_count * column_count;

		while (i)
		{
			weight_sum_squared += tiles[--i]->getWeight();
		}

		weight_sum_squared *= weight_sum_squared;
	}

	void TileMap::resetTileDrawing()
	{
		unsigned int i = row_count * column_count;

		while (i)
		{
			tiles[--i]->resetDrawing();
		}
	}
}  // namespace ufl_cap4053
