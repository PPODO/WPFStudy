using System;
using System.Collections.ObjectModel;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Windows;

namespace Chat.Content.SessionListPage.SessionList
{
    public class SessionInfo
    {
        #region Property

        public UInt32 SessionID
        {
            get; set;
        }

        public string SessionName
        {
            get; set;
        }

        public UInt16 JoinedUserCount
        {
            get; set;
        }
        public UInt16 MaxUserCount
        {
            get; set;
        }

        #endregion

        public SessionInfo(UInt32 sessionID, string sessionName, UInt16 joinedUserCount, UInt16 maxUserCount)
        {
            this.SessionID = sessionID;
            this.SessionName = sessionName;
            this.JoinedUserCount= joinedUserCount;
            this.MaxUserCount = maxUserCount;
        }
    }

    public class SessionList : ObservableCollection<SessionInfo>
    {
        public SessionList()
        {
        }

        public void AddNewSession(UInt32 sessionID, string sessionName, UInt16 joinedUserCount, UInt16 maxUserCount)
        {
            this.Add(new SessionInfo(sessionID, sessionName, joinedUserCount, maxUserCount));
        }

        public void RemoveSessionFromID(UInt32 sessionID)
        {
            foreach (var session in this.Items)
            {
                if (session.SessionID != sessionID) 
                    continue; 

                this.Remove(session);
                break;
            }
        }
    }
}
