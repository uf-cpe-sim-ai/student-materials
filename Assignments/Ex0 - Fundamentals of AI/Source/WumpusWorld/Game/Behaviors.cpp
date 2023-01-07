//! \file LeafBehaviors.h
//! \brief Defines the <code>fundamentals</code> leaf behavior classes.
//! \author Jeremiah Blanchard

#include "../Game/Agent.h"
#include "../Game/definitions.h"
#include "Behaviors.h"

#include <queue>
#include <stack>
#include <iostream>

namespace ufl_cap4053 { namespace fundamentals {

	bool Sequence::run(void (*dataFunction)(Behavior const*), void* context)
	{
		for (size_t index = 0; index < getChildCount(); index++)
			if (!((Sequence*)getChild(index))->run(dataFunction, context))
				return false;

		dataFunction(this);
		return true;
	}

	bool Selector::run(void (*dataFunction)(Behavior const*), void* context)
	{
		for (size_t index = 0; index < getChildCount(); index++)
		{
			if (((Selector*)getChild(index))->run(dataFunction, context))
			{
				dataFunction(this);
				return true;
			}
		}

		return false;
	}

	bool ProcessPercepts::run(void (*dataFunction)(Behavior const*), void* context)
	{
		Agent* agent = (Agent*) context;

		// Offsets for looking around a square.
		int offset[4][2] = { {-1, 0}, {1, 0}, {0, -1}, {0, 1} };

		// Local variables for working with the agent's knowledge.
		Knowledge& knowledge = agent->getKnowledge();
		unsigned x = knowledge.x, y = knowledge.y;
		vector<vector<char>>& stimulus = knowledge.stimuli;
		vector<vector<char>>& modelWorld = knowledge.modelWorld;

		// First, gather stimulus from the world state.
		bool breeze = ((stimulus[x][y] & BREEZE) != 0);
		bool stench = ((stimulus[x][y] & STENCH) != 0);

		// If there is no breeze or stench, then the boxes immediately around this square are clear.
		if (!breeze && !stench)
		{
			for (int index = 0; index < 4; index++)
			{
				unsigned newX = x + offset[index][0],
				         newY = y + offset[index][1];

				if (newX >= 0 && newX < modelWorld.size() && newY >= 0 && newY < modelWorld[newX].size())
				{
					modelWorld[newX][newY] = Knowledge::CLEAR;
				}
			}
		}
		
		// If there is a breeze here, there is a pit nearby. Update the agent's knowledge appropriately.
		if (breeze)
		{
			//Keep track of the non-pit spaces around this spot.
			int nonPitSpaces = 0;

			// First, count the non-pit spaces around the square.
			for (int index = 0; index < 4; index++)
			{
				unsigned newX = x + offset[index][0],
				         newY = y + offset[index][1];

				if (newX >= 0 && newX < modelWorld.size() && newY >= 0 && newY < modelWorld[newX].size())
				{
					// If the space cannot hold a pit, add it to the non-pit spaces.
					if (modelWorld[newX][newY] == Knowledge::CLEAR || modelWorld[newX][newY] == Knowledge::DEFINITE_WUMPUS)
						nonPitSpaces++;
				}
				else
					nonPitSpaces++; // If this is off of the map, it is not a pit space.
			}

			// Next, classify the space as best we can.
			for (int index = 0; index < 4; index++)
			{
				unsigned newX = x + offset[index][0],
				         newY = y + offset[index][1];

				if (newX >= 0 && newX < modelWorld.size() && newY >= 0 && newY < modelWorld[newX].size())
				{
					// If there is only one possible pit and this is it, mark it as such.
					if (nonPitSpaces == 3 && modelWorld[newX][newY] != Knowledge::CLEAR && modelWorld[newX][newY] != Knowledge::DEFINITE_WUMPUS)
					{
						modelWorld[newX][newY] = Knowledge::DEFINITE_PIT;
					}

					// If we believe that the space could hold a wumpus, mark it as possible wumpus OR pit.
					else if (modelWorld[newX][newY] == Knowledge::POSSIBLE_WUMPUS)
						modelWorld[newX][newY] = Knowledge::POSSIBLE_W_P;

					// If we know nothing about the space, note that it is possibly a pit. (All other cases are covered.)
					else if (modelWorld[newX][newY] == Knowledge::UNKNOWN)
						modelWorld[newX][newY] = Knowledge::POSSIBLE_PIT;
				}
			}
		}

		// If there is a stench and we have not yet fixed the location of the wumpus, we should do so now.
		if (stench && (knowledge.wumpusX == -1 || knowledge.wumpusY == -1))
		{
			//Keep track of the non-pit spaces around this spot.
			int nonWumpusSpaces = 0;

			// First, count the non-wumpus spaces around the square.
			for (int index = 0; index < 4; index++)
			{
				unsigned newX = x + offset[index][0],
				         newY = y + offset[index][1];

				if (newX >= 0 && newX < modelWorld.size() && newY >= 0 && newY < modelWorld[newX].size())
				{
					// If the space cannot hold the wumpus, add it to the non-wumpus spaces.
					if (modelWorld[newX][newY] == Knowledge::CLEAR || modelWorld[newX][newY] == Knowledge::DEFINITE_PIT)
						nonWumpusSpaces++;
				}
				else
					nonWumpusSpaces++; // If this is off of the map, it is not the wumpus space.
			}

			// Next, classify the space as best we can.
			for (int index = 0; index < 4; index++)
			{
				unsigned newX = x + offset[index][0],
				         newY = y + offset[index][1];

				if (newX >= 0 && newX < modelWorld.size() && newY >= 0 && newY < modelWorld[newX].size())
				{
					// If there is only one possible wumpus space and this is it, mark it as such.
					if (nonWumpusSpaces == 3 && modelWorld[newX][newY] != Knowledge::CLEAR && modelWorld[newX][newY] != Knowledge::DEFINITE_PIT)
					{
						modelWorld[newX][newY] = Knowledge::DEFINITE_WUMPUS;
						knowledge.wumpusX = newX;
						knowledge.wumpusY = newY;

						// Once we have found the wumpus, we can remove any other
						// "wumpus" marks from our knowledge of the world.
						for (unsigned xIndex = 0; xIndex < modelWorld.size(); xIndex++)
							for (unsigned yIndex = 0; yIndex < modelWorld[xIndex].size(); yIndex++)
							{
								if (modelWorld[xIndex][yIndex] == Knowledge::POSSIBLE_WUMPUS)
									modelWorld[xIndex][yIndex] = Knowledge::UNKNOWN;

								else if (modelWorld[xIndex][yIndex] == Knowledge::POSSIBLE_W_P)
									modelWorld[xIndex][yIndex] = Knowledge::POSSIBLE_PIT;
							}
					}

					// If we believe that the space could hold a pit, mark it as possible pit OR wumpus.
					else if (modelWorld[newX][newY] == Knowledge::POSSIBLE_PIT)
						modelWorld[newX][newY] = Knowledge::POSSIBLE_W_P;

					// If we know nothing about the space, note that it is possibly the wumpus. (All other cases are covered.)
					else if (modelWorld[newX][newY] == Knowledge::UNKNOWN)
						modelWorld[newX][newY] = Knowledge::POSSIBLE_WUMPUS;
				}
			}
		}

		// From this position, are there any safe, unexplored locations?
		// Or do we need to back track?

		knowledge.safeUnexploredLocationPresent = false;

		for (int index = 0; index < 4; index++)
		{
			unsigned newX = x + offset[index][0],
			         newY = y + offset[index][1];

			if (newX >= 0 && newX < modelWorld.size() && newY >= 0 && newY < modelWorld[newX].size())
			{
				if ((stimulus[newX][newY] & UNEXPLORED) && (modelWorld[newX][newY] == Knowledge::CLEAR))
				{
					knowledge.safeUnexploredLocationPresent = true;
					break;
				}
			}
		}

		dataFunction(this);
		return true;
	}

	bool DebugKnowledge::run(void (*dataFunction)(Behavior const*), void* context)
	{
		Knowledge& knowledge = ((Agent*)context)->getKnowledge();

		vector<vector<char>>& modelWorld = knowledge.modelWorld;
		vector<vector<char>>& stimulus = knowledge.stimuli;

		cout << "Agent World Model (With Stimuli)\n";
		for (unsigned int xIndex = 0; xIndex < knowledge.modelWorld.size(); xIndex++)
			cout << "------";
		cout << "-\n";

		for (unsigned int yIndex = 0; yIndex < knowledge.modelWorld[0].size(); yIndex++)
		{
			for (unsigned int xIndex = 0; xIndex < knowledge.modelWorld.size(); xIndex++)
			{
				cout << "| " << knowledge.getStateAsString(xIndex, yIndex) << " ";
			}
			cout << "|\n";
			for (unsigned int xIndex = 0; xIndex < knowledge.modelWorld.size(); xIndex++)
			{
				cout << "| " << knowledge.getStimuliAsString(xIndex, yIndex) << " ";
			}
			cout << "|\n";
			if (yIndex < knowledge.modelWorld[0].size() - 1)
			{
				for (unsigned int xIndex = 0; xIndex < knowledge.modelWorld.size(); xIndex++)
					cout << "|-----";
				cout << "|\n";
			}
			else
			{
				for (unsigned int xIndex = 0; xIndex < knowledge.modelWorld.size(); xIndex++)
					cout << "------";
				cout << "-\n\n";
			}
		}

		dataFunction(this);
		return true;
	}

		bool CheckForGold::run(void (*dataFunction)(Behavior const*), void* context)
	{
		Agent* agent = (Agent*) context;

		if (agent->getKnowledge().stimuli[agent->getKnowledge().x][agent->getKnowledge().y] & GOLD)
		{
			dataFunction(this);
			return true;
		}

		return false;
	}

	bool PickUpGold::run(void (*dataFunction)(Behavior const*), void* context)
	{
		Agent* agent = (Agent*) context;

		if (agent->pickUpGold())
		{
			dataFunction(this);
			return true;
		}

		return false;
	}

	bool ShootWumpus::run(void (*dataFunction)(Behavior const*), void* context)
	{
		Agent* agent = (Agent*) context;

		if ((agent->getKnowledge().stimuli[agent->getKnowledge().x][agent->getKnowledge().y] & STENCH) && (agent->shoot(LEFT)))
		{
			dataFunction(this);
			return true;
		}

		return false;
	}

	bool ExploreDirection::run(void (*dataFunction)(Behavior const*), void* context)
	{
		Agent* agent = (Agent*) context;

		Knowledge& knowledge = agent->getKnowledge();
		vector<vector<char>>& modelWorld = knowledge.modelWorld;
		vector<vector<char>>& stimulus = knowledge.stimuli;
		unsigned x = knowledge.x, y = knowledge.y;

		if (knowledge.safeUnexploredLocationPresent)
		{
			switch (direction)
			{
			case UP:
				if (y <= 0 || !(modelWorld[x][y-1] == Knowledge::CLEAR) || !(stimulus[x][y-1] & UNEXPLORED))
					return false;
				break;
			case DOWN:
				if (y >= modelWorld[0].size() - 1 || !(modelWorld[x][y+1] == Knowledge::CLEAR) || !(stimulus[x][y+1] & UNEXPLORED))
					return false;
				break;
			case LEFT:
				if (x <= 0 || !(modelWorld[x-1][y] == Knowledge::CLEAR) || !(stimulus[x-1][y] & UNEXPLORED))
					return false;
				break;
			case RIGHT:
				if (x >= modelWorld.size() - 1 || !(modelWorld[x+1][y] == Knowledge::CLEAR) || !(stimulus[x][y+1] & UNEXPLORED))
					return false;
				break;
			}
		}

		if (agent->move(direction))
		{
			dataFunction(this);
			return true;
		}

		return false;
	}

	bool TestBehavior::run(void (*dataFunction)(Behavior const*), void* context)
	{
		if (value)
		{
			dataFunction(this);
			return true;
		}

		return false;
	}

}}  // namespace ufl_cap4053::fundamentals
