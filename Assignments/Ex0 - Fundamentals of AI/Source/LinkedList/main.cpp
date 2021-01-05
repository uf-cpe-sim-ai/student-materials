#include "../Data Structures/LinkedList.h"

#include <limits>
#include <string>
#include <cassert>
#include <iostream>

using namespace std;
namespace fund = ufl_cap4053::fundamentals;

// Prototypes (allow implementation to follow main)
template <typename T, typename F>
void traverse(fund::LinkedList<T> const& q, F& dataFunction);

void printString(string testString);
void printCString(char const* testString);

int main()
{
    // Get ready.
    fund::LinkedList<string> valueList;
    fund::LinkedList<char const*> pointerList;

    unsigned int numOfStrings = 8;
    string testStrings[] = {
            "alpha"
            , "bravo"
            , "charlie"
            , "charlie"
            , "dog"
            , "echo"
            , "foxtrot"
            , "golf"
    };
    string tempStr;

    // Test isEmpty function.
    assert(pointerList.isEmpty() && valueList.isEmpty());

    // Test enqueue.
    cout << "Testing enqueue..." << endl;
    for (unsigned int index = 0; index < numOfStrings; ++index)
    {
        valueList.enqueue(testStrings[index]);
        pointerList.enqueue(testStrings[index].c_str());
        cout << "\tpointerList:";
        traverse(pointerList, printCString);
        cout << endl;
    }

    // Test dequeue.
    cout << endl << "Testing dequeue..." << endl;
    for (;;)
    {
        cout << "Removing the front element:";
        printCString(pointerList.getFront());
        pointerList.dequeue();
        cout << endl << "\tpointerList:";
        traverse(pointerList, printCString);
        cout << endl;

        if (pointerList.isEmpty())
            break;
        else
            valueList.dequeue();
    }

    // Test removal of only element.
    cout << endl << "Removing the only element from valueList:";
    printCString(valueList.getFront().c_str());
    valueList.remove(testStrings[numOfStrings - 1]);
    cout << endl << "\tvalueList:";
    traverse(valueList, printString);
    cout << endl;

    // Test containment check when empty
    cout << endl << "Testing search for: alpha";
    tempStr = "alpha";
    if (!valueList.contains(tempStr))
    {
        cout << endl << "\tvalueList:";
        traverse(valueList, printString);
        cout << endl;
    }

    // Test pop.
    cout << endl << "Testing pop..." << endl;
    cout << "Rebuilding list." << endl;
    for (unsigned int index = 0; index < numOfStrings; ++index)
        pointerList.enqueue(testStrings[index].c_str());

    cout << "\tpointerList:";
    traverse(pointerList, printCString);
    cout << endl;

    while (!pointerList.isEmpty())
    {
        cout << "Removing the back element:";
        printCString(pointerList.getBack());
        pointerList.pop();
        cout << endl << "\tpointerList:";
        traverse(pointerList, printCString);
        cout << endl;
    }

    // Test enqueue after removal.
    cout << endl << "Testing enqueue after removal..." << endl;
    for (unsigned int index = 1; index < numOfStrings; ++index)
    {
        valueList.enqueue(testStrings[index]);
        pointerList.enqueue(testStrings[index].c_str());
    }
    cout << "\tpointerList:";
    traverse(pointerList, printCString);
    cout << endl;

    // Test unsuccessful search.
    cout << endl << "Testing unsuccessful search and remove:";

    if (pointerList.contains(testStrings[0].c_str()))
    {
        assert(valueList.contains(testStrings[0]) && "alpha not found in valueList");
        cout << " " << testStrings[0];
        pointerList.remove(testStrings[0].c_str());
        valueList.remove(testStrings[0]);
        valueList.remove(testStrings[0]);
        cout << endl << "\tpointerList:";
        traverse(pointerList, printCString);
        cout << endl;
    }
    else
    {
        assert(!valueList.contains(tempStr) && "alpha found in valueList");
        cout << " ELEMENT NOT FOUND." << endl;
    }

    // Test removal of first element.
    cout << endl << "Testing search and remove:";

    if (pointerList.contains(testStrings[1].c_str()))
    {
        assert(valueList.contains(testStrings[1]) && "bravo not found in valueList");
        cout << " " << testStrings[1];
        pointerList.remove(testStrings[1].c_str());
        valueList.remove(testStrings[1]);
        valueList.remove(testStrings[1]);
        cout << endl << "\tpointerList:";
        traverse(pointerList, printCString);
        cout << endl;
    }
    else
    {
        assert(!valueList.contains(tempStr) && "bravo found in valueList");
        cout << " ELEMENT NOT FOUND." << endl;
    }

    // Test removal of middle element.
    cout << "Testing search and remove:";

    if (pointerList.contains(testStrings[4].c_str()))
    {
        assert(valueList.contains(testStrings[4]) && "dog not found in valueList");
        cout << " " << testStrings[4];
        pointerList.remove(testStrings[4].c_str());
        valueList.remove(testStrings[4]);
        valueList.remove(testStrings[4]);
        cout << endl << "\tpointerList:";
        traverse(pointerList, printCString);
        cout << endl;
    }
    else
    {
        assert(!valueList.contains(tempStr) && "dog found in valueList");
        cout << " ELEMENT NOT FOUND." << endl;
    }

    // Test removal of last element.
    cout << "Testing search and remove:";

    if (pointerList.contains(testStrings[7].c_str()))
    {
        assert(valueList.contains(testStrings[7]) && "golf not found in valueList");
        cout << " " << testStrings[7];
        pointerList.remove(testStrings[7].c_str());
        valueList.remove(testStrings[7]);
        valueList.remove(testStrings[7]);
        cout << endl << "\tpointerList:";
        traverse(pointerList, printCString);
        cout << endl;
    }
    else
    {
        assert(!valueList.contains(tempStr) && "golf found in valueList");
        cout << " ELEMENT NOT FOUND." << endl;
    }

    // Test removal of all elements by method.
    cout << endl << "Removing all elements from pointerList...";
    pointerList.clear();
    cout << endl << "\tpointerList:";
    traverse(pointerList, printCString);
    cout << endl;

    // Test destructor.
    cout << endl << "Adding elements to test descructor...";
    pointerList.enqueue("hotel");
    pointerList.enqueue("india");
    pointerList.enqueue("juliet");

    cout << endl << "\tpointerList:";
    traverse(pointerList, printCString);
    cout << endl << "\tvalueList:";
    traverse(valueList, printString);
    cout << endl << endl;

    cout << "Press ENTER to continue..." << endl;
    while(cin.get() != '\n') {;}
    return 0;
}

template <typename T, typename F>
void traverse(fund::LinkedList<T> const& q, F& dataFunction)
{
    printCString("[");

    for (auto itr = q.begin(); itr != q.end(); ++itr)
    {
        dataFunction(*itr);
    }
    printCString("]");
}

void printString(const string testString)
{
    cout << ' ' << testString;
}

void printCString(char const* testString)
{
    cout << ' ' << testString;
}
