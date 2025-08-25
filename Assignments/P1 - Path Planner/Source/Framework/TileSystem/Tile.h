//! \file Tile.h
//! \brief Defines the <code>ufl_cap4053::Tile</code> class.
//! \author Cromwell D. Enage, Jeremiah Blanchard
#pragma once

#include <vector>
#include "../../platform.h"

namespace ufl_cap4053 {

	//! \brief Logical representation of a tile in a hexagonal grid.
	//!
	//! Once a tile map is loaded, the application displays each tile as either an obstacle
	//! (if its weight is zero) or a white/gray hexagon (otherwise).  The smaller the weight,
	//! the lighter the color.
	class Tile;

	struct Line
	{
		Tile* destination;
		unsigned int line_color;
		Line* next;
		Line()
		{
			destination = nullptr;
			line_color = 0;
			next = nullptr;
		}
	};
	

	class Tile
	{
		friend class TileMap;

		unsigned char weight;
		int row;
		int column;
		double x, y;

		// For drawing purposes
		unsigned int marker_color, outline_color, fill_color;
		std::vector<std::pair<const Tile*,unsigned>> lines;

		Tile(int r, int c, double radius, unsigned char data);
		~Tile();
		void setRadius(double radius);

		// Converts between SBGR and LRGB
		inline int convertColorModel(int color)
		{
			unsigned char first = ~(color >> 24);
			unsigned char second = color;
			unsigned char third = (color >> 8);
			unsigned char fourth = (color >> 16);

			return ((int)first << 24) | ((int)second << 16) | ((int)third << 8) | (int)fourth;
		}

		inline void resetDrawing()
		{
			marker_color = outline_color = fill_color = 0;
			lines.clear();
		}

	public:
		//! \brief Returns the terrain weight of this tile, or zero if this tile is impassable.
		inline unsigned char getWeight() const
		{
			return weight;
		}

		//! \brief Returns the row-coordinate of this location.
		//!
		//! Use this method when determining if a tile is adjacent to another tile.
		inline int getRow() const
		{
			return row;
		}

		//! \brief Returns the column-coordinate of this location.
		//!
		//! Use this method when determining if a tile is adjacent to another tile.
		inline int getColumn() const
		{
			return column;
		}

		//! \brief Returns the x-coordinate of this location.
		//!
		//! Use this method when calculating the various costs of a search node.
		inline double getXCoordinate() const
		{
			return x;
		}

		//! \brief Returns the y-coordinate of this location.
		//!
		//! Use this method when calculating the various costs of a search node.
		inline double getYCoordinate() const
		{
			return y;
		}

		//! \brief Returns the LRGB marker color of this tile.
		inline unsigned int getMarker() const
		{
			return marker_color;
		}

		//! \brief Sets this tile's marker color to the designated color in the LRGB color space.
		inline void setMarker(unsigned int color)
		{
			marker_color = convertColorModel(color);
		}

		//! \brief Returns the LRGB outline color of this tile.
		inline unsigned int getOutline() const
		{
			return outline_color;
		}

		//! \brief Sets this tile's outline color to the designated color in the LRGB color space.
		inline void setOutline(unsigned int color)
		{
			outline_color = convertColorModel(color);
		}

		//! \brief Returns the LRGB fill color of this tile.
		inline unsigned int getFill() const
		{
			return fill_color;
		}

		//! \brief Sets this tile's fill color to the designated color in the LRGB color space.
		inline void setFill(unsigned int color)
		{
			fill_color = convertColorModel(color);
		}

		//! \brief Returns the vector of lines being drawn from this tile.
		inline const std::vector<std::pair<const Tile*,unsigned>>& getLines() const
		{
			return lines;
		}

		//! \brief Adds a line to be drawn from this tile to destination using the designated
		//! color in the LRGB color space if a line is not already being drawn to the destination
		//! from this tile.
		inline void addLineTo(Tile* destination, unsigned int color)
		{
			lines.push_back(std::pair<const Tile*, unsigned>(destination, convertColorModel(color)));
		}

		//! \brief Removes existing lines from the drawing set.
		inline void clearLines()
		{
			lines.clear();
		}
	};
}  // namespace ufl_cap4053

