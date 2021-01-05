//! \file LeafBehaviors.h
//! \brief Defines the <code>fundamentals</code> leaf behavior classes.
//! \author Jeremiah Blanchard
#pragma once

#include "../BehaviorTree/Behavior.h"

namespace ufl_cap4053 { namespace fundamentals {

	//! \brief C++ implementation of sequence for behavior trees.
	class Sequence : public Behavior
	{
	public:
		//! \brief Creates a new <code>%Sequence</code>with the description.
		Sequence(char const* _description) : Behavior(_description) { }

		//! \brief Executes the behavior. Returns true (and runs dataFunction) on success, false otherwise.
		//!
		//! \param   dataFunction  a single-argument function that accepts the behavior as 
		//!                        a valid argument.
		//!
		//! \pre     <code>NULL !=</code> \a this
		bool run(void (*dataFunction)(Behavior const*), void* context);
	};

	//! \brief C++ implementation of Selector behavior for behavior trees.
	class Selector : public Behavior
	{
	public:
		//! \brief Creates a new <code>%Selector</code>with the description.
		Selector(char const* _description) : Behavior(_description) { }

		//! \brief Executes the behavior. Returns true (and runs dataFunction) on success, false otherwise.
		//!
		//! \param   dataFunction  a single-argument function that accepts the behavior as 
		//!                        a valid argument.
		//!
		//! \pre     <code>NULL !=</code> \a this
		bool run(void (*dataFunction)(Behavior const*), void* context);
	};

	//! \brief C++ implementation of a leaf node in a behavior tree.
	class ProcessPercepts : public Behavior
	{
	public:
		ProcessPercepts(char const* _description) : Behavior(_description) {}
		bool run(void (*dataFunction)(Behavior const*), void* context);
		bool isLeaf() const { return true; }
	};

	//! \brief C++ implementation of a leaf node in a behavior tree.
	class DebugKnowledge : public Behavior
	{
	public:
		DebugKnowledge(char const* _description) : Behavior(_description) {}
		bool run(void (*dataFunction)(Behavior const*), void* context);
		bool isLeaf() const { return true; }
	};

	//! \brief C++ implementation of a leaf node in a behavior tree.
	class CheckForGold : public Behavior
	{
	public:
		CheckForGold(char const* _description) : Behavior(_description) {}
		bool run(void (*dataFunction)(Behavior const*), void* context);
		bool isLeaf() const { return true; }
	};

	//! \brief C++ implementation of a leaf node in a behavior tree.
	class PickUpGold : public Behavior
	{
	public:
		PickUpGold(char const* _description) : Behavior(_description) {}
		bool run(void (*dataFunction)(Behavior const*), void* context);
		bool isLeaf() const { return true; }
	};

	//! \brief C++ implementation of a leaf node in a behavior tree.
	class ShootWumpus : public Behavior
	{
	public:
		ShootWumpus(char const* _description) : Behavior(_description) {}
		bool run(void (*dataFunction)(Behavior const*), void* context);
		bool isLeaf() const { return true; }
	};

	//! \brief C++ implementation of a leaf node in a behavior tree.
	class ExploreDirection : public Behavior
	{
	private:
		Direction direction;

	public:
		ExploreDirection(char const* _description, Direction _direction) : Behavior(_description), direction(_direction) {}
		bool run(void (*dataFunction)(Behavior const*), void* context);
		bool isLeaf() const { return true; }
	};
	
		//! \brief C++ implementation of a leaf node in a behavior tree (this one does nothing; just a place holder.)
	class TestBehavior : public Behavior
	{
	private:
		bool value;
	public:
		TestBehavior(char const* _description, bool _value) : Behavior(_description), value(_value) {}
		bool run(void (*dataFunction)(Behavior const*), void* context);
		bool isLeaf() const { return true; }
	};

}}  // namespace ufl_cap4053::fundamentals

