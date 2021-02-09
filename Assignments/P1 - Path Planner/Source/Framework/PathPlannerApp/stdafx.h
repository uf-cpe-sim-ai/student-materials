// Code by Cromwell D. Enage
// August 2010
#ifndef _STDAFX_H_
#define _STDAFX_H_

//! \brief Use this document as a reference manual.
//!
//! All core components used by the search algorithms are defined in this namespace.
//!
//! If a function or method specification includes one or more preconditions, then your code
//! must fulfill these preconditions before the function or method can be used.  If a function or
//! method specification includes one or more postconditions, then any code you write to implement
//! this function must ultimately fulfill these postconditions.
namespace ufl_cap4053 {
}  // namespace ufl_cap4053

//! \brief Brush up on your knowledge of the standard template library here.
//!
//! The three types of data structures that your code must manage in this lab are the
//! <code>vector</code>, the <code>pair</code>, and the <code>map</code>.  Mastering them
//! is of paramount importance.
//!
//! The <code>vector</code> and <code>deque</code> class templates have almost identical
//! interfaces, the main exception being that you can add to and remove from the front
//! of a <code>deque</code>.  The <code>deque</code> can be useful if you are implementing
//! a pure breadth-first search algorithm.
namespace std {

	//! \brief Definition of an unsigned integer type.
	typedef unsigned int size_t;

	//! \brief Simplified documentation for the STL
	//! <a href="http://www.sgi.com/tech/stl/Vector.html"><code>%vector</code></a> class template.
	//!
	//! There are two basic ways of dealing with a <code>%vector</code>.  The usual way is to
	//! treat it as a runtime-resizable array, or a
	//! <a href="http://www.sgi.com/tech/stl/RandomAccessSequence.html">random-access sequence</a>
	//! in STL parlance.
	//!
	//! \code
	//! std::vector<char const*> names;
	//!
	//! names.resize(6);
	//! names[0] = "alpha";
	//! names[1] = "bravo";
	//! names[2] = "charlie";
	//! names[3] = "dog";
	//! names[4] = "charlie";
	//! names[5] = "foxtrot";
	//!
	//! // Find all instances of "charlie".
	//! for (std::size_t i = 0; i < names.size(); ++i)
	//! {
	//!     if (names[i] == "charlie")
	//!     {
	//!         // Do something...
	//!     }
	//! }
	//! \endcode
	//!
	//! The more generic (code-reusable) way is to treat it as a
	//! <a href="http://www.sgi.com/tech/stl/BackInsertionSequence.html">back-insertion
	//! sequence</a>--where you can add and remove elements to and from the back--and as a
	//! <a href="http://www.sgi.com/tech/stl/Container.html">container</a> whose elements you can
	//! access via <a href="http://www.sgi.com/tech/stl/Iterators.html">iterators</a>.
	//!
	//! \code
	//! std::vector<char const*> names;
	//!
	//! names.push_back("alpha");
	//! names.push_back("bravo");
	//! names.push_back("charlie");
	//! names.push_back("dog");
	//! names.push_back("charlie");
	//! names.push_back("foxtrot");
	//!
	//! // Find all instances of "charlie".
	//! for (std::vector<char const*>::iterator itr = names.begin(); itr != names.end(); ++itr)
	//! {
	//!     if (*itr == "charlie")
	//!     {
	//!         // Do something...
	//!     }
	//! }
	//! \endcode
	//!
	//! The methods documented here are the ones you are most likely to use in this lab.  For
	//! complete documentation, visit the <a href="http://www.sgi.com/tech/stl/">official
	//! website</a>.
	template <typename T>
	class vector
	{
	public:
		//! \brief The type of an object that points to an element in this container.
		typedef implementation_defined iterator;

		//! \brief Returns <code>true</code> if there are no elements left in this container,
		//! <code>false</code> otherwise.
		bool empty() const;

		//! \brief Removes all elements from this container.
		//!
		//! \note
		//!   - If each element is a pointer to heap-allocated memory, then this method does
		//!     <em>not</em> clean up that memory.  You must do this yourself beforehand.
		//!
		//! \post
		//!   - <code>empty()</code>
		void clear();

		//! \brief Returns the number of elements in this container.
		size_t size() const;

		//! \brief Resizes this container to hold the specified number of elements.
		//!
		//! \note
		//!   - If the type <code>T</code> is a pointer type, then this method neither
		//!     heap-allocates memory nor cleans it up.  However, if the previous size
		//!     was smaller than the one specified here, then the additional elements
		//!     will all be <code>NULL</code>.
		//!
		//! \post
		//!   - <code>size() ==</code> \a size
		void resize(size_t size);

		//! \brief Indexing operator.
		//!
		//! Returns the element at the specified index.
		//!
		//! \pre
		//!   - \a index <code>\< size()</code>
		T& operator[](size_t index);

		//! \brief Returns an iterator pointing to the first element in this container.
		//!
		//! Returns <code>end()</code> if this container is empty.
		iterator begin();

		//! \brief Returns an iterator pointing past-the-end of this container.
		iterator end();

		//! \brief Inserts the specified element before the element pointed at by the specified
		//! iterator.
		//!
		//! For example, let our <code>%vector</code> of <code>names</code> hold these strings:
		//! <code>"alpha"</code>, <code>"bravo"</code>, <code>"charlie"</code>, <code>"dog"</code>,
		//! <code>"echo"</code>, and <code>"foxtrot"</code>.  The following code inserts the string
		//! <code>"golf"</code> into the <code>names</code> container at position 2.
		//!
		//! \code
		//!     names.insert(names.begin() + 2, "golf");
		//! \endcode
		//!
		//! Our names container will hold: <code>"alpha"</code>, <code>"bravo"</code>,
		//! <code>"golf"</code>, <code>"charlie"</code>, <code>"dog"</code>, <code>"echo"</code>,
		//! and <code>"foxtrot"</code>. 
		iterator insert(iterator pos, T const& element);

		//! \brief Returns the last element in this container.
		//!
		//! \pre
		//!   - <code>! empty()</code>
		T& back();

		//! \brief Appends the specified element to the back of this container.
		//!
		//! \post
		//!   - <code>back() ==</code> \a element
		//!   - <code>! empty()</code>
		void push_back(T& element);

		//! \brief Removes the last element from this container.
		//!
		//! \note
		//!   - If the element is a pointer to heap-allocated memory, then this method does
		//!     <em>not</em> clean up that memory.
		//!
		//! \pre
		//!   - <code>! empty()</code>
		void pop_back();
	};

	//! \brief Simplified documentation for the STL
	//! <a href="http://www.sgi.com/tech/stl/Deque.html"><code>%deque</code></a> class template.
	//!
	//! The <code>%deque</code> combines the random-access abilities of the <code>vector</code>
	//! with the double-ended access/modification abilities of the
	//! <a href="http://www.sgi.com/tech/stl/List.html"> <code>list</code></a>.  It is the default
	//! underlying <a href="http://www.sgi.com/tech/stl/Sequence.html">sequence</a> of the
	//! <a href="http://www.sgi.com/tech/stl/stack.html"><code>stack</code></a> and
	//! <a href="http://www.sgi.com/tech/stl/queue.html"><code>queue</code></a> container adaptors,
	//! because it avoids potential memory reallocations that occur when a <code>vector</code> is
	//! used, while using less memory per element than a <code>list</code>.  The trade-off is that
	//! the random access of an element in a <code>%deque</code> takes slightly longer than that
	//! of an element in a <code>vector</code>.
	//!
	//! \code
	//! std::deque<char const*> names;
	//!
	//! names.push_front("charlie");
	//! names.push_front("bravo");
	//! names.push_front("alpha");
	//! names.push_back("dog");
	//! names.push_back("charlie");
	//! names.push_back("foxtrot");
	//!
	//! // Find all instances of "charlie" using indices.
	//! for (std::size_t i = 0; i < names.size(); ++i)
	//! {
	//!     if (names[i] == "charlie")
	//!     {
	//!         // Do something...
	//!     }
	//! }
	//!
	//! // Find all instances of "charlie" using iterators.
	//! for (std::deque<char const*>::iterator itr = names.begin(); itr != names.end(); ++itr)
	//! {
	//!     if (*itr == "charlie")
	//!     {
	//!         // Do something...
	//!     }
	//! }
	//!
	//! // Remove all names, one at a time, alternating from either end.
	//! while (!names.empty())
	//! {
	//!     if (names.size() % 2)
	//!     {
	//!         names.pop_front();
	//!     }
	//!     else
	//!     {
	//!         names.pop_back();
	//!     }
	//! }
	//! \endcode
	//!
	//! The methods documented here are the ones you are most likely to use in this lab.  For
	//! complete documentation, visit the <a href="http://www.sgi.com/tech/stl/">official
	//! website</a>.
	template <typename T>
	class deque
	{
	public:
		//! \brief The type of an object that points to an element in this container.
		typedef implementation_defined iterator;

		//! \brief Returns <code>true</code> if there are no elements left in this container,
		//! <code>false</code> otherwise.
		bool empty() const;

		//! \brief Removes all elements from this container.
		//!
		//! \note
		//!   - If each element is a pointer to heap-allocated memory, then this method does
		//!     <em>not</em> clean up that memory.  You must do this yourself beforehand.
		//!
		//! \post
		//!   - <code>empty()</code>
		void clear();

		//! \brief Returns the number of elements in this container.
		size_t size() const;

		//! \brief Resizes this container to hold the specified number of elements.
		//!
		//! \note
		//!   - If the type <code>T</code> is a pointer type, then this method neither
		//!     heap-allocates memory nor cleans it up.  However, if the previous size
		//!     was smaller than the one specified here, then the additional elements
		//!     will all be <code>NULL</code>.
		//!
		//! \post
		//!   - <code>size() ==</code> \a size
		void resize(size_t size);

		//! \brief Indexing operator.
		//!
		//! Returns the element at the specified index.
		//!
		//! \pre
		//!   - \a index <code>\< size()</code>
		T& operator[](size_t index);

		//! \brief Returns an iterator pointing to the first element in this container.
		//!
		//! Returns <code>end()</code> if this container is empty.
		iterator begin();

		//! \brief Returns an iterator pointing past-the-end of this container.
		iterator end();

		//! \brief Returns the first element in this container.
		//!
		//! \pre
		//!   - <code>! empty()</code>
		T& front();

		//! \brief Appends the specified element to the front of this container.
		//!
		//! \post
		//!   - <code>front() ==</code> \a element
		//!   - <code>! empty()</code>
		void push_front(T& element);

		//! \brief Removes the first element from this container.
		//!
		//! \note
		//!   - If the element is a pointer to heap-allocated memory, then this method does
		//!     <em>not</em> clean up that memory.
		//!
		//! \pre
		//!   - <code>! empty()</code>
		void pop_front();

		//! \brief Returns the last element in this container.
		//!
		//! \pre
		//!   - <code>! empty()</code>
		T& back();

		//! \brief Appends the specified element to the back of this container.
		//!
		//! \post
		//!   - <code>back() ==</code> \a element
		//!   - <code>! empty()</code>
		void push_back(T& element);

		//! \brief Removes the last element from this container.
		//!
		//! \note
		//!   - If the element is a pointer to heap-allocated memory, then this method does
		//!     <em>not</em> clean up that memory.
		//!
		//! \pre
		//!   - <code>! empty()</code>
		void pop_back();
	};

	//! \brief Simplified documentation for the STL
	//! <a href="http://www.sgi.com/tech/stl/pair.html"><code>%pair</code></a> class template.
	//!
	//! Though a useful data structure in its own right, the STL <code>%pair</code> mainly works
	//! together with the <code>map</code> class template by holding a key as its first element
	//! and the corresponding value as its second element.
	//!
	//! The members documented here are the ones you are most likely to use in this lab.  For
	//! complete documentation, visit the <a href="http://www.sgi.com/tech/stl/">official
	//! website</a>.
	template <typename T1, typename T2>
	class pair
	{
	public:
		//! \brief The first element.
		//!
		//! In a <code>map</code>, this would hold the key in a key-value <code>%pair</code>.
		T1 first;

		//! \brief The second element.
		//!
		//! In a <code>map</code>, this would hold the value in a key-value <code>%pair</code>.
		T2 second;

		//! \brief Constructs an empty <code>%pair</code> object.
		pair();

		//! \brief Constructs a new <code>%pair</code> object holding the specified elements.
		pair(T1 t1, T2 t2);
	};

	//! \brief Simplified documentation for the STL
	//! <a href="http://www.sgi.com/tech/stl/Map.html"><code>%map</code></a> class template.
	//!
	//! The following example requires a user-defined type.
	//!
	//! \code
	//! struct FloridayKey
	//! {
	//!     // public member variables and functions...
	//! };
	//! \endcode
	//!
	//! By default, the <code>%map</code> class template can store pointers to objects of
	//! user-defined types as either keys or values.  The example code below illustrates
	//! the typical usage of such a <code>%map</code>.
	//!
	//! \code
	//! std::map<FloridayKey*,char const*> floridaMap;
	//!
	//! // Heap-allocate some keys.
	//! FloridaKey* islamorada = new FloridaKey();
	//! FloridaKey* keyLargo = new FloridaKey();
	//! FloridaKey* keyWest = new FloridaKey();
	//! FloridaKey* marathon = new FloridaKey();
	//! FloridaKey* bigPineKey = new FloridaKey();
	//!
	//! // One way to insert key-value pairs.
	//! floridaMap.insert(std::map<FloridayKey*,char const*>::value_type(islamorada, "Islamorada"));
	//! floridaMap.insert(std::map<FloridayKey*,char const*>::value_type(keyLargo, "Key Largo"));
	//!
	//! // A more convenient way to insert key-value pairs.
	//! floridaMap[keyWest] = "Key West";
	//! floridaMap[marathon] = "Marathon";
	//!
	//! // One way to search for key-value pairs by key.
	//! std::map<FloridayKey*,char const*>::iterator itr = floridaMap.find(islamorada);
	//!
	//! if (itr != floridaMap.end())
	//! {
	//!     std::cout << itr->second << std::endl;
	//! }
	//!
	//! // Another way to search for key-value pairs by key.  Use it if you will insert another
	//! // pair with the exact same key when it has not been originally found.
	//! char const* name = floridaMap[bigPineKey];
	//!
	//! if (name == NULL)
	//! {
	//!     // Perform key-value pair insertion here.
	//! }
	//! else
	//! {
	//!     std::cout << name << std::endl;
	//! }
	//!
	//! // You must use iterators to go through each key-value pair in the map.
	//! // DO NOT USE A LOOP JUST TO SEARCH FOR A SPECIFIC KEY!
	//! for (
	//!     std::map<FloridayKey*,char const*>::iterator itr = floridaMap.begin();
	//!     itr != floridaMap.end();
	//!     ++itr
	//! )
	//! {
	//!		// Print out all values.
	//!     std::cout << itr->second << std::endl;
	//! }
	//! \endcode
	//!
	//! The methods documented here are the ones you are most likely to use in this lab.  For
	//! complete documentation, visit the <a href="http://www.sgi.com/tech/stl/">official
	//! website</a>.
	template <typename K, typename V>
	class map
	{
	public:
		//! \brief The type of an object that points to a key-value <code>pair</code> in this
		//! container.
		typedef implementation_defined iterator;

		//! \brief The type of the object returned when dereferencing a valid iterator.
		typedef pair<K const,V> value_type;

		//! \brief Returns <code>true</code> if there are no key-value pairs left in this
		//! container, <code>false</code> otherwise.
		bool empty() const;

		//! \brief Removes all key-value pairs from this container.
		//!
		//! \note
		//!   - If each key-value <code>pair</code> stores pointers to heap-allocated memory, then
		//!     this method does <em>not</em> clean up tha memory.  You must do this yourself
		//!     beforehand.
		//!
		//! \post
		//!   - <code>empty()</code>
		void clear();

		//! \brief Returns an iterator pointing to the first key-value <code>pair</code> in this
		//! container.
		//!
		//! Returns <code>end()</code> if this container is empty.
		iterator begin();

		//! \brief Returns an iterator pointing past-the-end of this container.
		iterator end();

		//! \brief Inserts the specified key-value pair into this container.
		//!
		//! For the purposes of this lab, you can ignore the return value.
		pair<iterator,bool> insert(value_type const& key_value_pair);

		//! \brief Finds the key-value <code>pair</code> whose first element matches the specified
		//! key, and returns the second element.
		//!
		//! If the type <code>V</code> is a pointer type, returns <code>NULL</code> if the key is
		//! not found.  This results in a new <code>pair(key, NULL)</code> being inserted into
		//! this container.
		V& operator[](K const& key);

		//! \brief Returns an iterator pointing to the key-value <code>pair</code> whose first
		//! element matches the specified key.
		//!
		//! Returns <code>end()</code> if the key is not found.
		iterator find(K const& key);
	};
}  // namespace std

//! \page legal Legal Information
//!
//! \section copyright Copyright
//!
//! (c) 2010 Full Sail University, (c) 2019 University of Florida
//!
//! \section license MIT License
//!
//! Permission is hereby granted, free of charge, to any person obtaining a copy of this software
//! and associated documentation files (the "Software"), to deal in the Software without
//! restriction, including without limitation the rights to use, copy, modify, merge, publish,
//! distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the
//! Software is furnished to do so, subject to the following conditions:
//!
//! The above copyright notice and this permission notice shall be included in all copies or
//! substantial portions of the Software.
//!
//! THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING
//! BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
//! NONINFRINGEMENT.  IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
//! DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//! OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//!
//! Except as contained in this notice, the name(s) of the above copyright holders shall not be
//! used in advertising or otherwise to promote the sale, use or other dealings in this Software
//! without prior written authorization.

#endif  // _STDAFX_H_

