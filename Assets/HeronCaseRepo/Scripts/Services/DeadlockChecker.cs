using System;
using System.Collections.Generic;
using UnityEngine;

namespace HeronCaseRepo.Scripts.Services
{
    public class DeadlockChecker : IDisposable
    {
        private List<TubeView> _allTubes;
        
        public DeadlockChecker()
        {
            EventBus<PourCompletedEvent>.Subscribe(OnPourCompleted);
        }
        
        public void Initialize(List<TubeView> tubes)
        {
            _allTubes = tubes;
        }
        
        public void Dispose()
        {
            EventBus<PourCompletedEvent>.Unsubscribe(OnPourCompleted);
        }

        private void OnPourCompleted(PourCompletedEvent e)
        {
            for (var i = 0; i < _allTubes.Count; i++)
            {
                for (var j = i + 1; j < _allTubes.Count; j++)
                {
                    if (_allTubes[j].IsSolved)
                    {
                        continue;
                    }
                    
                    if (_allTubes[i].TopColor == _allTubes[j].TopColor &&
                        _allTubes[j].TopColor == _allTubes[i].TopColor &&
                        _allTubes[j].AvailableSlots < _allTubes[i].TopColorCount &&
                        _allTubes[i].AvailableSlots < _allTubes[j].TopColorCount)
                    {
                        DeadlockFound();
                        goto DeadlockFound;
                    }
                }
            }
            DeadlockFound:;
        }

        private static void DeadlockFound()
        {
            EventBus<DeadlockEvent>.Publish(new DeadlockEvent {});
        }
    }
    
}