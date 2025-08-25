//! \file World.h
//! \brief Defines the <code>fundamentals::World</code> class.
//! \author Jeremiah Blanchard
#pragma once

#include "definitions.h"
#include <vector>

using namespace std;

namespace ufl_cap4053 { namespace fundamentals {

	class World
	{
		friend class Game;

	private:
		vector<vector<char>> stimulus;
		int width, height;
		int agentX, agentY;

		bool agentAlive;
		bool wumpusAlive;
		bool goldRetrieved;
		bool agentHasArrow;

	public:
		// Constructor
		World(char** _stimulus, unsigned _width, unsigned _height);

		// Get methods
		char getStimulus();
		unsigned getWidth();
		unsigned getHeight();

		// Agent commands
		bool moveAgent(Direction);
		bool retrieveGold();

		void attackWumpus(Direction);
	};

}}  // namespace ufl_cap4053::fundamentals
