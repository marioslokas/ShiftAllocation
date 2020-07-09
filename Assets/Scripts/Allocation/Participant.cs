using System.Linq;
using UnityEngine;

namespace Allocation
{
    public enum DayConstrains
    {
        CantDoFirstDay,
        CantDoSecondDay,
        CantDoThirdDay,
        CantDoFourthDay,
        CantDoFifthDay,
        CantDoSixthDay,
        CantDoSeventhDay,
        CantDoEightDay
    }
    
    /// <summary>
    /// Class for representing participants
    /// </summary>
    public class Participant
    {
        public string name;
        public static int numberOfShifts = 3;
        private int shiftsTaken;

        public int ShiftsTaken
        {
            get => shiftsTaken;
            set => shiftsTaken = value;
        }

        private DayConstrains[] personConstrains;
        
        public Participant(string name, DayConstrains[] constrains = null)
        {
            if (constrains == null)
            {
                personConstrains = new DayConstrains[0];
            }
            else
            {
                personConstrains = constrains;
            }
            
            this.name = name;
        }

        public bool HasConstrains()
        {
            return personConstrains.Any();
        }

        public bool IsDone()
        {
            return shiftsTaken >= numberOfShifts;
        }

        // checks if the person can do that day
        public bool CanDo(int dayNumber)
        {
            // check if the person has already reached max
            if (shiftsTaken == numberOfShifts)
                return false;
            
            DayConstrains thisDay = (DayConstrains) dayNumber;
            if (personConstrains.Contains(thisDay)) 
                return false;
            return true;
        }
        
    }
}
