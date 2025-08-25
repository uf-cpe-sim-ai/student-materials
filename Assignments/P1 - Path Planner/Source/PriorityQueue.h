//! \file PriorityQueue.h
//! \brief Defines the <code>ufl_cap4053::PriorityQueue</code> class template.
//! \author Cromwell D. Enage, Jeremiah Blanchard
#pragma once

#include <vector>
#include <deque>
#include <algorithm>

namespace ufl_cap4053 {

	//! \brief The open heap used by all cost-based search algorithms.
	//!
	//! This class template is basically a thin wrapper on top of both the <code>std::deque</code>
	//! class template and the <a href="http://www.sgi.com/tech/stl/">STL</a> heap operations.
	template <typename T>
	class PriorityQueue
	{
		std::deque<T> open;
		bool (*compare)(T const&, T const&);

	public:
		//! \brief Constructs a new <code>%PriorityQueue</code> that heap-sorts nodes
		//! using the specified comparator.
		explicit PriorityQueue(bool (*)(T const&, T const&));

		//! \brief Returns <code>true</code> if the heap contains no nodes,
		//! <code>false</code> otherwise.
		bool empty() const;

		//! \brief Removes all nodes from the heap.
		//!
		//! \post
		//!   - <code>empty()</code>
		void clear();

		//! \brief Returns the number of nodes currently in the heap.
		std::size_t size() const;

		//! \brief Pushes the specified node onto the heap.
		//!
		//! The heap will maintain the ordering of its nodes during this operation.
		//!
		//! \post
		//!   - <code>! empty()</code>
		void push(T const& node);

		//! \brief Returns the least costly node in the heap.
		//!
		//! \pre
		//!   - <code>! empty()</code>
		T front() const;

		//! \brief Removes the least costly node from the heap.
		//!
		//! The heap will maintain the ordering of its nodes during this operation.
		//!
		//! \pre
		//!   - <code>! empty()</code>
		void pop();

		//! \brief Removes all instances of the specified node from the heap.
		void remove(T const& node);

		//! \brief Enumerates all nodes in the heap so far.
		//!
		//! \param   sorted  the container to which each node will be added.
		//!
		//! \pre
		//!   - There must be no <code>NULL</code> pointers in the heap.
		//! \post
		//!   - All nodes should be sorted by this heap's comparator.
		void enumerate(std::vector<T>& sorted) const;
	};

	template <typename T>
	PriorityQueue<T>::PriorityQueue(bool (*c)(T const&, T const&)) : open(), compare(c)
	{
	}

	template <typename T>
	bool PriorityQueue<T>::empty() const
	{
		return open.empty();
	}

	template <typename T>
	void PriorityQueue<T>::clear()
	{
		open.clear();
	}

	template <typename T>
	std::size_t PriorityQueue<T>::size() const
	{
		return open.size();
	}

	template <typename T>
	void PriorityQueue<T>::push(T const& node)
	{
		open.insert(std::upper_bound(open.begin(), open.end(), node, compare), node);
	}

	template <typename T>
	T PriorityQueue<T>::front() const
	{
		return open.back();
	}

	template <typename T>
	void PriorityQueue<T>::pop()
	{
		open.pop_back();
	}

	template <typename T>
	void PriorityQueue<T>::remove(T const& node)
	{
		open.erase(std::remove(open.begin(), open.end(), node), open.end());
	}

	template <typename T>
	void PriorityQueue<T>::enumerate(std::vector<T>& sorted) const
	{
		sorted.resize(open.size());
		std::copy(open.begin(), open.end(), sorted.begin());
	}
}  // namespace ufl_cap4053

#ifdef _WIN32
#include <windows.h>
#include <tchar.h>
#define printf MessageBox(0,_T("printf is not permitted"), _T("PLEASE USE cout <<"), 0);
#define system MessageBox(0,_T("system is not permitted"), _T("YIPE!"), 0);
#endif
