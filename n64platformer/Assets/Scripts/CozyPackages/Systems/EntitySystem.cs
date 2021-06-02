using System;

using UnityEngine;
using com.cozyhome.Singleton;
using System.Collections.Generic;

namespace com.cozyhome.Systems
{
    public interface IEntity
    {
        void OnInsertion();
        void OnRemoval();
    }

    public class EntitySystem<T> : SingletonBehaviour<EntitySystem<T>> where T : MonoBehaviour, IEntity
    {
        private List<T> _entities = null;
        public void RegisterEntities() => _entities = null;
        public void RegisterEntities(T[] _arrents) 
        { 
            _entities = new List<T>();

            foreach(T _ent in _arrents)
            {
                _entities.Add(_ent); 
                _ent.OnInsertion();
            }
        }
        public void RegisterEntities(List<T> _listents) 
        { 
            _entities = new List<T>();

            foreach(T _ent in _listents)
            {
                _entities.Add(_ent);
                _ent.OnInsertion();
            }
        }

        public void UnregisterAllEntities() 
        {
            for(int i = EntityCount - 1;i>=0;i--)
            {
                _entities.RemoveAt(i);
                _entities[i].OnRemoval();
            }
        }

        public void RegisterEntity(T _ent) { _entities.Add(_ent); _ent.OnInsertion(); }
        public void UnregisterEntity(T _ent) { _entities.Remove(_ent); _ent.OnRemoval(); }

        public void ActUponAtIndex(Action<T> _action, int _idx) =>
            _action.Invoke(_entities[_idx % EntityCount]);

        public void ActUponAllEntities(Action<T> _action)
        {
            for(int i = EntityCount - 1;i >= 0;i--)
                _action.Invoke(_entities[i]);
        }

        // Only access this if you know what you're doing.
        public List<T> Entities => _entities;

        public int EntityCount => _entities.Count;
    }
}

