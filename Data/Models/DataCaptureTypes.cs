﻿namespace Shisaku_no_Hashi.Data.Models
{
    public class DataCaptureTypes
    {
        public class PostData
        {
            public string Account { get; set; }
            public string Username { get; set; }
            public string SessionId { get; set; }
            public string Universe { get; set; }
            public string Locale { get; set; }
            public string SceneId { get; set; }
            public string Lobby { get; set; }
            public List<EventData> Events { get; set; }
        }

        public class EventData
        {
            public EventTypes EventType { get; set; }
            public DateTime StartTime { get; set; }
            public DateTime? EndTime { get; set; }
            public ObjectData Object { get; set; }
        }

        public class ObjectData
        {
            public string Id { get; set; }
            public string Content { get; set; }
            public string? Instance { get; set; }
        }

        public enum EventTypes
        {
            SessionStart = 1001,                // 0x3E9
            EnterScene = 11000,                 // 0x2AF8
            ChangeClothing = 20101,             // 0x4E85
            ChangeAppearance = 20201,           // 0x4EE9
            AddPurchase = 30101,                // 0x7595
            AddReward = 30102,                  // 0x7596
            ObjectPlace = 30201,                // 0x75F9
            ObjectUse = 40101,                  // 0x9CA5
            ScreenZoom = 40201,                 // 0x9D09
            ScreenLink = 40202,                 // 0x9D0A
            WorldChat = 40301,                  // 0x9D6D
            Dance = 41001,                      // 0xA029
            CreateClan = 50101,                 // 0xC3B5
            JoinClan = 50102,                   // 0xC3B6
            LeaveClan = 50103,                  // 0xC3B7
            ExpelledFromClan = 50104,           // 0xC3B8
            DisbandClan = 50105,                // 0xC3B9
            NetworkError = 61881,               // 0xF1B9
            SecurityErrorSignIn = 990000,       // 0xF1B30
            SecurityErrorObjectDefinition = 990001, // 0xF1B31
            SecurityErrorObjectDescription = 990002, // 0xF1B32
            SecurityErrorSceneArchive = 990003, // 0xF1B33
            SecurityErrorSceneDescription = 990004, // 0xF1B34
            //SecurityErrorDNSCheck = 99005 ?
            OnNetworkError = 990099,                   // 0xF1B93
        }
    }
}