﻿using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RobotInterrogation.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RobotInterrogation.Services
{
    public class InterviewService
    {
        private static ConcurrentDictionary<int, Interview> Interviews = new ConcurrentDictionary<int, Interview>();

        private static int NextInterviewID = 1;
        private static object IdLock = new object();

        private GameConfiguration Configuration { get; }

        public InterviewService(IOptions<GameConfiguration> configuration)
        {
            Configuration = configuration.Value;
        }

        public string GetNextInterviewID()
        {
            int id;

            lock (IdLock)
            {
                id = NextInterviewID++;
            }

            Interviews[id] = new Interview();

            return id.ToString();
        }

        public bool TryAddUser(Interview interview, string connectionID)
        {
            if (interview.Status != InterviewStatus.WaitingForConnections)
                return false;

            if (interview.InterviewerConnectionID == null)
            {
                interview.InterviewerConnectionID = connectionID;
                return true;
            }

            if (interview.SuspectConnectionID == null)
            {
                interview.SuspectConnectionID = connectionID;
                return true;
            }

            return false;
        }

        public void RemoveInterview(string interviewID)
        {
            int id = int.Parse(interviewID);
            if (!Interviews.TryRemove(id, out Interview interview))
                return;

            if (interview.Status == InterviewStatus.InProgress)
            {
                LogInterview(interview);
            }
        }

        public Interview GetInterview(string interviewID)
        {
            int id = int.Parse(interviewID);

            if (!Interviews.TryGetValue(id, out Interview interview))
                throw new Exception($"Invalid interview ID: {interviewID}");

            return interview;
        }

        public Interview GetInterviewWithStatus(string interviewID, InterviewStatus status)
        {
            var interview = GetInterview(interviewID);

            if (interview.Status != status)
                throw new Exception($"Interview doesn't have the required status {status} - it is actually {interview.Status}");

            return interview;
        }

        private void AllocateRandomValues<T>(IList<T> source, IList<T> destination, int targetNum)
        {
            destination.Clear();

            var random = new Random();

            while (destination.Count < targetNum)
            {
                int iSelection = random.Next(source.Count);
                T selection = source[iSelection];

                if (destination.Contains(selection))
                    continue;

                destination.Add(selection);
            }
        }

        public void AllocatePenalties(Interview interview)
        {
            AllocateRandomValues(Configuration.Penalties, interview.Penalties, 3);
        }

        public string[] GetAllPackets()
        {
            return Configuration.Packets
                .Select(p => p.Name)
                .ToArray();
        }

        public Packet GetPacket(int index)
        {
            return Configuration.Packets[index];
        }

        public void AllocateRoles(Interview interview)
        {
            AllocateRandomValues(interview.Packet.Roles, interview.Roles, 3);
        }

        public void AllocateQuestions(Interview interview)
        {
            AllocateRandomValues(interview.Packet.PrimaryQuestions, interview.PrimaryQuestions, 2);
            AllocateRandomValues(interview.Packet.SecondaryQuestions, interview.SecondaryQuestions, 2);
        }

        public void AllocateSuspectNotes(Interview interview)
        {
            AllocateRandomValues(Configuration.SuspectNotes, interview.SuspectNotes, 2);
        }

        public InterviewOutcome GuessSuspectRole(Interview interview, bool guessIsRobot)
        {
            var actualRole = interview.Roles[0].Type;
            InterviewOutcome outcome;

            if (guessIsRobot)
            {
                outcome = actualRole == SuspectRoleType.Human
                    ? InterviewOutcome.WronglyGuessedRobot
                    : InterviewOutcome.CorrectlyGuessedRobot;
            }
            else
            {
                outcome = actualRole == SuspectRoleType.Human
                    ? InterviewOutcome.CorrectlyGuessedHuman
                    : InterviewOutcome.WronglyGuessedHuman;
            }

            interview.Status = InterviewStatus.Finished;
            interview.Outcome = outcome;

            LogInterview(interview);

            return outcome;
        }

        public void KillInterviewer(Interview interview)
        {
            if (interview.Roles[0].Type != SuspectRoleType.ViolentRobot)
            {
                throw new Exception("Suspect is not a violent robot, so cannot kill interviewer");
            }

            interview.Status = InterviewStatus.Finished;
            interview.Outcome = InterviewOutcome.KilledInterviewer;

            LogInterview(interview);
        }

        public Interview ResetInterview(string interviewID)
        {
            var oldInterview = GetInterviewWithStatus(interviewID, InterviewStatus.Finished);

            int id = int.Parse(interviewID);

            var newInterview = new Interview();
            Interviews[id] = newInterview;

            newInterview.Status = InterviewStatus.SelectingPositions;
            newInterview.InterviewerConnectionID = oldInterview.InterviewerConnectionID;
            newInterview.SuspectConnectionID = oldInterview.SuspectConnectionID;

            return newInterview;
        }

        private void LogInterview(Interview interview)
        {
            // TODO: save this data to somewhere here ... but not the connection IDs
        }
    }
}
