using System.Collections;
using System;
using System.Linq;
using Unity.Netcode;
using System.Collections.Generic;
using UnityEngine;

public class TournamentData: MonoBehaviour
{

}

//[System.Serializable]
//public class TournamentAData : ISaveLoadData, INetworkSerializable
//{
//    public enum tournamentStage
//    {
//        QuarterFinal,
//        SemiFinal,
//        Final
//    }
    

//    public tournamentStage CurrentStage => _stage;
//    private tournamentStage _stage;

//    public bool isStarted => _started;
//    private bool _started;

//    public TournamentAData(tournamentStage stage = tournamentStage.QuarterFinal, bool started = false)
//    {
//        _stage = stage;
//        _started = started;
//    }

//    public void SetDefaultValues()
//    {
//        _stage = tournamentStage.QuarterFinal;
//        _started = false;
//    }

//    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
//    {
//        serializer.SerializeValue(ref _stage);
//    }
//}


//[System.Serializable]
//public class TournamentBData : ISaveLoadData, INetworkSerializable
//{
//    public enum tournamentStage
//    {
//        QuarterFinal,
//        SemiFinal,
//        Final
//    }
    

//    public tournamentStage CurrentStage => _stage;
//    private tournamentStage _stage;

//    public bool isStarted => _started;
//    private bool _started;

//    public TournamentBData(tournamentStage stage = tournamentStage.QuarterFinal, bool started = false)
//    {
//        _stage = stage;
//        _started = started;
//    }

//    public void SetDefaultValues()
//    {
//        _stage = tournamentStage.QuarterFinal;
//        _started = false;
//    }

//    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
//    {
//        serializer.SerializeValue(ref _stage);
//    }
//}
