//! \file World.cpp
//! \brief Implements the <code>fundamentals::World</code> class.
//! \author Jeremiah Blanchard

#include <queue>
#include <stack>
#include "World.h"

namespace ufl_cap4053 { namespace fundamentals {

	// Constructor
	World::World(char** _stimulus, unsigned _width, unsigned _height)
	{
		width = _width;
		height = _height;
		stimulus.resize(width);

		for (int xIndex = 0; xIndex < width; xIndex++)
		{
			stimulus[xIndex].resize(height);

			for (int yIndex = 0; yIndex < height; yIndex++)
			{
				stimulus[xIndex][yIndex] = _stimulus[xIndex][yIndex];

				if (stimulus[xIndex][yIndex] & START)
				{
					agentX = xIndex;
					agentY = yIndex;
				}
			}
		}

		agentAlive = true;
		wumpusAlive = true;
		goldRetrieved = false;
		agentHasArrow = true;
	}

	// Get methods
	char World::getStimulus()
	{
		return stimulus[agentX][agentY];
	}
	
	unsigned World::getWidth()
	{
		return width;
	}

	unsigned World::getHeight()
	{
		return height;
	}

	// Agent actions
	bool World::moveAgent(Direction direction)
	{
		bool success = false;
		// TODO
		switch (direction)
		{
		case UP:
			if (agentY > 0)
			{
				agentY--;
				success = true;
			}
			break;

		case DOWN:
			if (agentY < height - 1)
			{
				agentY++;
				success = true;
			}
			break;

		case LEFT:
			if (agentX > 0)
			{
				agentX--;
				success = true;
			}
			break;

		case RIGHT:
			if (agentX < width - 1)
			{
				agentX++;
				success = true;
			}
			break;
		}

		if ((stimulus[agentX][agentY] & WUMPUS) || (stimulus[agentX][agentY] & PIT))
			agentAlive = false;

		return success;
	}

	bool World::retrieveGold()
	{
		if (stimulus[agentX][agentY] & GOLD)
		{
			stimulus[agentX][agentY] ^= GOLD;
			goldRetrieved = true;
			return true;
		}

		return false;
	}

	void World::attackWumpus(Direction direction)
	{
		if (!agentHasArrow)
			return;

		agentHasArrow = false;

		switch (direction)
		{
		case UP:
			if (agentY > 0 && stimulus[agentX][agentY-1] & WUMPUS)
			{
				stimulus[agentX][agentY-1] ^= WUMPUS;
				wumpusAlive = false;
			}
			return;
		case DOWN:
			if (agentY < height - 1 && stimulus[agentX][agentY+1] & WUMPUS)
			{
				stimulus[agentX][agentY+1] ^= WUMPUS;
				wumpusAlive = false;
			}
			return;
		case LEFT:
			if (agentX > 0 && stimulus[agentX-1][agentY] & WUMPUS)
			{
				stimulus[agentX-1][agentY] ^= WUMPUS;
				wumpusAlive = false;
			}
			return;
		case RIGHT:
			if (agentY < height - 1 && stimulus[agentX+1][agentY] & WUMPUS)
			{
				stimulus[agentX+1][agentY] ^= WUMPUS;
				wumpusAlive = false;
			}
			return;
		}
	}

}}  // namespace ufl_cap4053::fundamentals
