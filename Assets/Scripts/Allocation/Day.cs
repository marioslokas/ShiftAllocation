using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Allocation
{
    public class Day
    {
        private int dayNumber;

        public int DayNumber => dayNumber;

        private Shift breakfastShift;
        private Shift dinnerShift;

        private const int breakfastNoOfWorkers = 2;
        public static int dinnerNoOfWorkers;
    
        public Day(int dayNumber, bool shouldHaveBreakfast, bool shouldHaveDinner)
        {
            this.dayNumber = dayNumber;
            breakfastShift = shouldHaveBreakfast ? new Shift(breakfastNoOfWorkers, false) : 
                new Shift(breakfastNoOfWorkers, true);
            dinnerShift = shouldHaveDinner ? new Shift(dinnerNoOfWorkers, false) : 
                new Shift(dinnerNoOfWorkers, true);
        }

        public bool IsDayFilled()
        {
            if (!dinnerShift.IsShiftAvailable())
                return true;
            
            return false;
        }

        // picks random not filled shift and assigns
        public Shift GetAvailableShift()
        {
            if (breakfastShift.IsShiftAvailable()) return breakfastShift;
            else if (dinnerShift.IsShiftAvailable() ) return dinnerShift;

            return null;
        }

        public void LogAssignments()
        {
//            Debug.Log("Breakfast shift on Day " + dayNumber + " is: ");
//            breakfastShift.LogAssignments();
            Debug.Log("Dinner shift on Day " +dayNumber + " is: ");
            dinnerShift.LogAssignments(dayNumber);
        }
        
        public string GetAssignmentsString()
        {
            StringBuilder myAssignmentsString = new StringBuilder();
            myAssignmentsString.Append("Day Number :" + dayNumber + "\n");
            myAssignmentsString.Append(dinnerShift.GetAssignmentsString());
            return myAssignmentsString.ToString();
        }
        
        
        
        public class Shift
        {
            private bool notAvailableShift;
            
            private int numberOfSpots;
            private int spotsIndex;

            private Person[] assignedPersons;
        
            public Shift(int numberOfSpots, bool notAvailableShift)
            {
                this.notAvailableShift = notAvailableShift;
                
                this.numberOfSpots = numberOfSpots;
                assignedPersons = new Person[numberOfSpots];
            }

            public bool IsShiftAvailable()
            {
                if (notAvailableShift)
                    return false;
                
                if (spotsIndex >= numberOfSpots)
                {
                    return false;
                }

                return true;
            }

            public bool Assign(Person thisPerson, int dayNumber)
            {
                if (!IsShiftAvailable())
                {
                    Debug.Log("Spots filled for this day");
                    return false;
                }

                assignedPersons[spotsIndex] = thisPerson;
                thisPerson.ShiftsTaken++;
                spotsIndex++;

                return true;
            }

            public bool PersonIsAssignedToShift(Person thisPerson)
            {
                if (assignedPersons.Contains(thisPerson))
                {
                    return true;
                }

                return false;
            }

            public void LogAssignments(int dayNumber)
            {
                for (int i = 0; i < numberOfSpots; i++)
                {
                    Debug.Log("Day Number : " + dayNumber + " " + assignedPersons[i].name);
                }
            }

            public string GetAssignmentsString()
            {
                string assignmentsString = String.Empty;
                for (int i = 0; i < numberOfSpots; i++)
                {
                    assignmentsString += assignedPersons[i].name + "\n";
                }

                return assignmentsString;
            }

            
        }
    }
}
