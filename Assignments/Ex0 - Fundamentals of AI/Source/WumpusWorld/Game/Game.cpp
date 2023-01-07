// Game.cpp - the entry point of the Behavior Tree Game (in the Wumpus World.)
// Code by Jeremiah Blanchard, August 2009
// Updated by Jeremiah Blanchard, January 2012
#include <limits>
#include <iostream>
#include <vector>
#include <queue>

#ifdef _WIN32
#include "Windows.h"
#endif

#include "definitions.h"
#include "Game.h"
#include "Behaviors.h"

using namespace std;
using namespace ufl_cap4053::fundamentals;

char world1[6][6] = { { NONE, STENCH, WUMPUS, STENCH, NONE, NONE },
					  { NONE, NONE, BREEZE | STENCH | GOLD, NONE, NONE, NONE },
					  { NONE, BREEZE, PIT, BREEZE, NONE, BREEZE },
					  { NONE, NONE, BREEZE, BREEZE, BREEZE, PIT },
					  { NONE, NONE, BREEZE, PIT, BREEZE, BREEZE },
					  { START, NONE, NONE, BREEZE, NONE, NONE } };

Behavior* buildTree();
void deleteTree(Behavior* root);
void logBehavior(Behavior const* behavior);
void printBehavior(const Behavior* behavior);
void printBehavior(const char* data);
string getBehaviorLog();

namespace ufl_cap4053 { namespace fundamentals {

	void Game::main()
	{
		char** worldData = new char*[6];
		for (int xIndex = 0; xIndex < 6; xIndex++)
		{
			worldData[xIndex] = new char[6];
			for (int yIndex = 0; yIndex < 6; yIndex++)
				worldData[xIndex][yIndex] = world1[xIndex][yIndex];
		}

		World world(worldData, 6, 6);
		Behavior *behavior = buildTree();

		//		ProcessPercepts* behavior = new ProcessPercepts("Process Percepts");
		Agent agent(world, *behavior, logBehavior);
		agent.enter(world.agentX, world.agentY);

		cout << "\nWorld Information\n-----------------\n";
		cout << "Agent Position: (" << world.agentX << ", " << world.agentY << ")" << endl;
		cout << "Agent Has Arrow: " << world.agentHasArrow << endl;
		cout << "Agent Has Gold: " << world.goldRetrieved << endl;
		cout << "Agent is Alive: " << world.agentAlive << endl;
		cout << "Wumpus is Alive: " << world.wumpusAlive << endl << endl;

		while(world.agentAlive && world.agentHasArrow)
		{
			agent.update();
			cout << "Leaf Behaviors\n--------------\n";
			cout << getBehaviorLog();

			cout << "\nWorld Information\n-----------------\n";
			cout << "Agent Position: (" << world.agentX << ", " << world.agentY << ")" << endl;
			cout << "Agent Has Arrow: " << world.agentHasArrow << endl;
			cout << "Agent Has Gold: " << world.goldRetrieved << endl;
			cout << "Agent is Alive: " << world.agentAlive << endl;
			cout << "Wumpus is Alive: " << world.wumpusAlive << endl << endl;
		}

		if (!world.agentAlive)
			cout << "You died!" << endl;

		if (world.goldRetrieved)
			cout << "You found the gold!" << endl;

		if (!world.wumpusAlive)
			cout << "You killed the wumpus!" << endl;

		cout << "Press ENTER to continue..." << endl;
		while(cin.get() != '\n') {;}

		deleteTree(behavior);
		for (unsigned index = 0; index < 6; index++)
			delete[] worldData[index];

		delete[] worldData;
	}
}}

int main()
{
#ifdef _WIN32
	SetConsoleOutputCP(CP_UTF8);
#endif

	// First, run a general test of the behavior tree mechanisms.
	Behavior* root = buildTree();
	cout << "\nBreadth-First:\n--------------\n";
	root->breadthFirstTraverse(printBehavior);
	cout << "\nPreorder\n--------\n";
	root->preOrderTraverse(printBehavior);
	cout << "\nPostorder\n---------\n";
	root->postOrderTraverse(printBehavior);
	deleteTree(root);

	cout << "Press ENTER to continue..." << endl;
	while(cin.get() != '\n') {;}

	// Then, run the wumpus world game simulation.
	ufl_cap4053::fundamentals::Game::main();
}

Behavior* buildTree()
{
	Behavior* behavior = new Sequence("Basic Behavior");
	behavior->addChild(new ProcessPercepts("Process Percepts"));
	behavior->addChild(new DebugKnowledge("Debug Knowledge"));
	behavior->addChild(new Selector("Choose Action"));
	behavior->getChild(2)->addChild(new Sequence("Look For Gold"));
	behavior->getChild(2)->getChild(0)->addChild(new CheckForGold("Check For Gold"));
	behavior->getChild(2)->getChild(0)->addChild(new PickUpGold("Pick Up Gold"));
	behavior->getChild(2)->addChild(new ShootWumpus("Shoot Wumpus"));
	behavior->getChild(2)->addChild(new Selector("Explore"));
	behavior->getChild(2)->getChild(2)->addChild(new ExploreDirection("Explore Up", UP));
	behavior->getChild(2)->getChild(2)->addChild(new ExploreDirection("Explore Down", DOWN));
	behavior->getChild(2)->getChild(2)->addChild(new ExploreDirection("Explore Left", LEFT));
	behavior->getChild(2)->getChild(2)->addChild(new ExploreDirection("Explore Right", RIGHT));
	return behavior;
}

void deleteTree(Behavior* root)
{
	std::queue<Behavior*> q;
	q.push(root);

	while (!q.empty())
	{
		Behavior* current = q.front();
		q.pop();

		for (size_t index = 0; index < current->getChildCount(); index++)
			q.push(current->getChild(index));

		delete current;
	}
}

void printBehavior(const char *data)
{
	cout << data << endl;
}

void printBehavior(const Behavior* behavior)
{
	cout << behavior->toString() << endl;
}

string behaviorLog = "";

void logBehavior(const Behavior* behavior)
{
	if (behavior->isLeaf())
		behaviorLog = behaviorLog + behavior->toString() + "\n";
}

string getBehaviorLog()
{
	string log = behaviorLog;
	behaviorLog = "";
	return log;
}