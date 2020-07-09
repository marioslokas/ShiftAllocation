using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Allocation
{
    public static class Extensions
    {
        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
        {
            System.Random rand = new System.Random();
            return source.OrderBy((item) => rand.Next());
        }
    }
    
    public class Calculation : MonoBehaviour
    {
        [Header("Parameters for calculation")]
        [SerializeField] private int numberOfDays;
        [SerializeField] private int numberOfPeoplePerShift;
        [SerializeField] private int personMaxShifts;

        [SerializeField] private TMP_InputField[] namesInputFields;
        [SerializeField] private Text[] daysResults;

        [SerializeField] private string personWhoCantDoAllDays;
        [SerializeField] private DayConstrains[] cantDoDays;
        
        private Participant[] participants;
        private Participant[] participantsWithConstrains; // do those first
        private Participant[] normalParticipants;
        
        private int numberOfParticipants; // namesInputFields size of array
        private Day[] days;

        private bool checkPeople;

        private float timeCount;

        private void Awake()
        {
            Day.dinnerNoOfWorkers = numberOfPeoplePerShift;
            Participant.numberOfShifts = personMaxShifts;
        }
    

        private void Start()
        {
            Clear();
        }

        private void Clear()
        {
            timeCount = 0;
            
            numberOfParticipants = namesInputFields.Length;
            
            participants = new Participant[numberOfParticipants];
            days = new Day[numberOfDays];
            
            for (int j = 0; j < numberOfDays; j++)
            {
                days[j] = new Day(j, false, true);   
            } 
        }

        private bool CheckEndCondition()
        {
            if (checkPeople)
            {
                return (
                    normalParticipants.Any(x => !x.IsDone()) && participantsWithConstrains.Any(x=> !x.IsDone()));
            }
            else
            {
                return days.Any(x => !x.IsDayFilled());
            }
        }

        public void Calculate()
        {
            Clear();
            
            for (int i = 0; i < numberOfParticipants; i++)
            {
                string name = namesInputFields[i].text;
                
                if (name.Equals(personWhoCantDoAllDays))
                {
                    participants[i] = new Participant(name, cantDoDays); 
                }
                else
                {
                    participants[i] = new Participant(name);
                }
                
            }
            

            List<Day> notFilledDays;
            List<Day> notFilledDaysParticipantCanDo;
            List<Day> notFilleDaysParticipantCanDoAndNotAlreadySignedUpFor;

            if (numberOfPeoplePerShift * numberOfDays >= personMaxShifts * participants.Length) // more shifts than people
            {
                checkPeople = true;
                Debug.Log("More shifts than people");
            }
            else // more people than shifts
            {
                Debug.Log("More people than shifts");
                checkPeople = false;
            }

            participantsWithConstrains = participants.Where(x => x.HasConstrains()).ToArray();
            normalParticipants = participants.Where(x => !x.HasConstrains()).ToArray();

            int contraintsCounter = 0;
            Participant[] constrainedParticipantsNotDone = 
                participantsWithConstrains.Where(x => !x.IsDone()).Shuffle().ToArray();

            int normalCounter = 0;
            Participant[] participantsNotDone = normalParticipants.Where(x => !x.IsDone()).Shuffle().ToArray();

            while (CheckEndCondition())
            {
                // check for end
                timeCount += 1;

                if (timeCount > 10000)
                {
                    Debug.LogError("ERROR: Max iterations reached.");
                    break;
                }

                Participant participant;
                
                if (participantsWithConstrains.Any(x=> !x.IsDone()) )
                {
                    participant = constrainedParticipantsNotDone[contraintsCounter];
                    contraintsCounter++;
                }
                else
                {
                    participant = participantsNotDone[normalCounter];
                    normalCounter++;
                }
                
                // reset and reshuffle arrays
                if (contraintsCounter >= participantsWithConstrains.Length)
                {
                    contraintsCounter = 0; 
                    constrainedParticipantsNotDone = 
                        participantsWithConstrains.Where(x => !x.IsDone()).Shuffle().ToArray();
                }

                if (normalCounter >= participantsNotDone.Length)
                {
                    normalCounter = 0;
                    participantsNotDone = normalParticipants.Where(x => !x.IsDone()).Shuffle().ToArray();
                }
                                
                
                if (participant == null) break;
                
                Day notFilledDay;
                
                // any days not filled?
                notFilledDays = days.Where(x => !x.IsDayFilled()).ToList();
                // any days participant can do?
                notFilledDaysParticipantCanDo = notFilledDays.Where(x => participant.CanDo(x.DayNumber)).ToList();
                // days they haven't already signed up for?
                notFilleDaysParticipantCanDoAndNotAlreadySignedUpFor
                    = notFilledDaysParticipantCanDo
                        .Where(x => !x.GetAvailableShift().PersonIsAssignedToShift(participant)).ToList();
                
                // skip participant if the only days left are ones he signed up for
                if (notFilleDaysParticipantCanDoAndNotAlreadySignedUpFor.Count == 0) continue;

                int r = Random.Range(0, notFilleDaysParticipantCanDoAndNotAlreadySignedUpFor.Count);
                notFilledDay = notFilleDaysParticipantCanDoAndNotAlreadySignedUpFor[r];
                
                Day.Shift availableShift = notFilledDay.GetAvailableShift();
                availableShift.Assign(participant, notFilledDay.DayNumber);

            }
            
            // print the ones who don't do more than 2 shifts
            participantsNotDone = normalParticipants.Where(x => !x.IsDone()).Shuffle().ToArray();
            
            Debug.Log("Participants who are still not done:");
            
            for (int i = 0; i < participantsNotDone.Length; i++)
            {
                Debug.Log("Participant: " + 
                          participantsNotDone[i].name + " Number of shifts: " + participantsNotDone[i].ShiftsTaken);
            }
            
            Debug.Log("Participants who do 3 shifts: ");

            Participant[] participantsDone = participants.Where(x => x.IsDone()).ToArray();
            
            for (int i = 0; i < participantsDone.Length; i++)
            {
                Debug.Log("Participant: " + 
                          participantsDone[i].name + " Number of shifts: " + participantsDone[i].ShiftsTaken);
            }

            for (int i = 0; i < daysResults.Length; i++)
            {
                daysResults[i].text = days[i].GetAssignmentsString();
            }
            
        }
    }
}