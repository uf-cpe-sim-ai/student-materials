#include <cmath>

#include "Tile.h"

using namespace std;  // Because the sqrt() function really belongs here.

namespace ufl_cap4053 {

	Tile::Tile(int r, int c, double radius, unsigned char data)
		: weight(data), row(r), column(c)
		, x(((r & 1) ? ((c + 1) << 1) : ((c << 1) | 1)) * radius)
		, y((r * 3 + 2) * radius / sqrt(3.0))
		, marker_color(0), outline_color(0), fill_color(0)
	{
	}
	Tile::~Tile()
	{
		clearLines();
	}
	void Tile::setRadius(double radius)
	{
		x = ((row & 1) ? ((column + 1) << 1) : ((column << 1) | 1)) * radius;
		y = (row * 3 + 2) * radius / sqrt(3.0);
	}
}  // namespace ufl_cap4053

