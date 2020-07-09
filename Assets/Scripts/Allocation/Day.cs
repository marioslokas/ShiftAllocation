using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Allocation
{
    /// <summary>
    /// Represents one day with shifts
    /// </summary>
    public class Day
    {
        private int dayNumber;

        public int DayNumber => dayNumber;

        private Shift dinnerShift;

        private const int breakfastNoOfWorkers = 2;
        public static int dinnerNoOfWorkers;
    
        public Day(int dayNumber, bool shouldHaveBreakfast, bool shouldHaveDinner)
        {
            this.dayNumber = dayNumber;
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
            if (dinnerShift.IsShiftAvailable() ) return dinnerShift;
            return null;
        }
        
        public string GetAssignmentsString()
        {
            StringBuilder myAssignmentsString = new StringBuilder();
            myAssignmentsString.Append("Day Number :" + dayNumber + "\n");
            myAssignmentsString.Append(dinnerShift.GetAssignmentsString());
            return myAssignmentsString.ToString();
        }
        
        
        /// <summary>
        /// Class for holding shift info and reference to people assigned
        /// </summary>
        public class Shift
        {
            private bool notAvailableShift;
            private int numberOfSpots;
            private int spotsIndex;

            private Participant[] _assignedParticipants;
        
            public Shift(int numberOfSpots, bool notAvailableShift)
            {
                this.notAvailableShift = notAvailableShift;
                
                this.numberOfSpots = numberOfSpots;
                _assignedParticipants = new Participant[numberOfSpots];
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

            public void Assign(Participant thisParticipant, int dayNumber)
            {
                if (!IsShiftAvailable())
                {
                    Debug.Log("Spots filled for this day");
                    return;
                }

                _assignedParticipants[spotsIndex] = thisParticipant;
                thisParticipant.ShiftsTaken++;
                spotsIndex++;
            }

            public bool PersonIsAssignedToShift(Participant thisParticipant)
            {
                if (_assignedParticipants.Contains(thisParticipant))
                {
                    return true;
                }

                return false;
            }

            public void LogAssignments(int dayNumber)
            {
                for (int i = 0; i < numberOfSpots; i++)
                {
                    Debug.Log("Day Number : " + dayNumber + " " + _assignedParticipants[i].name);
                }
            }

            public string GetAssignmentsString()
            {
                string assignmentsString = String.Empty;
                for (int i = 0; i < numberOfSpots; i++)
                {
                    assignmentsString += _assignedParticipants[i].name + "\n";
                }

                return assignmentsString;
            }

            
        }
    }
}
