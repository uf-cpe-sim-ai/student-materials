//! \file Behavior.h
//! \brief Defines the <code>fundamentals::Behavior</code> abstract class.
//! \author Jeremiah Blanchard with code from Cromwell D. Enage
#pragma once

#include "../../Data Structures/TreeNode.h"
#include <vector>

namespace ufl_cap4053 { namespace fundamentals {

	//! \brief C++ implementation of an n-ary behavior tree node.
	class Behavior : TreeNode<const char*>
	{
	private:
		static unsigned createdCount;
		static unsigned destroyedCount;

	public:
		static unsigned getCreatedCount();
		static unsigned getDestroyedCount();

		//=================
		//  BASIC METHODS
		//=================

		//! \brief Creates a new <code>%Behavior</code>with the description.
		Behavior(char const* _description);

		//! \brief Returns a string representation of this <code>%Behavior</code>.
		char const* toString() const { return TreeNode::getData(); }

		//! \brief Returns a true if the behavior is a leaf node and false otherwise. (Default: false.)
		virtual bool isLeaf() const { return false; }

		std::size_t getChildCount() const { return TreeNode::getChildCount(); }
		const char* getData() { return TreeNode::getData(); }

		//! \brief Returns a pointer to the index-th child node stored in this
		//! <code>%Behavior</code>.
		//!
		//! \pre      \a index <code>\< getChildCount()</code>
		//! \post     <code>NULL != getChild(index)</code>
		Behavior* getChild(size_t index) { return (Behavior*)TreeNode::getChild(index); }
		Behavior const* getChild(size_t index) const { return (Behavior const*)TreeNode::getChild(index); }
		
		//! \brief Removes and returns a node at specified index from the children of this <code>%TreeNode</code>.
		//!
		//! \pre      \a index <code>\< getChildCount()</code>
		Behavior* removeChild(std::size_t index) { return (Behavior*) TreeNode::removeChild(index); }

		//! \brief Adds a child to the child behaviors of this <code>%Behavior</code>.
		//!
		//! \pre      \a child <code>\!= NULL</code>
		void addChild(Behavior* child) { TreeNode::addChild(child); }

		//=====================
		//  TRAVERSAL METHODS
		//=====================

		//! \brief Traverses the root and all sub-nodes breadth-first.
		//!
		//! \param   dataFunction  a single-argument function that accepts the traversed node
		//!                        a valid argument.
		//!
		//! \pre     <code>NULL !=</code> \a this
		void breadthFirstTraverse(void (*dataFunction)(const char*)) { TreeNode<const char*>::breadthFirstTraverse(dataFunction); }

		//! \brief Traverses the root and all sub-nodes in pre-order fashion.
		//!
		//! \param   dataFunction  a single-argument function that accepts the traversed node
		//!                        a valid argument.
		//!
		//! \pre     <code>NULL !=</code> \a this
		void preOrderTraverse(void (*dataFunction)(const char *)) { TreeNode<const char*>::preOrderTraverse(dataFunction); }

		//! \brief Traverses the root and all sub-nodes in post-order fashion.
		//!
		//! \param   dataFunction  a single-argument function that accepts the traversed node
		//!                        a valid argument.
		//!
		//! \pre     <code>NULL !=</code> \a this
		void postOrderTraverse(void (*dataFunction)(const char*)) { TreeNode<const char*>::postOrderTraverse(dataFunction); }

		//=======================================
		//  VIRTUAL (BEHAVIOR-SPECIFIC) METHODS
		//=======================================

		//! \brief Executes the behavior. Returns true (and runs dataFunction) on success, false otherwise.
		//!
		//! \param   dataFunction  a single-argument function that accepts the behavior as 
		//!                        a valid argument.
		//!
		//! \pre     <code>NULL !=</code> \a this
		virtual bool run(void (*dataFunction)(const Behavior*), void* context) = 0;
	};
}}  // namespace ufl_cap4053::fundamentals
