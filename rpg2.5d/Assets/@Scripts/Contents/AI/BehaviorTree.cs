using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.ShaderGraph.Drawing.Inspector.PropertyDrawers;
using UnityEngine;

namespace BehaviorTree
{
    public interface IBTNode
    {
        public enum ENodeState
        {
            Success,
            Failure,
            Running
        }

        public ENodeState Evaluate();
    }


    public sealed class ActionNode : IBTNode
    {
        Func<IBTNode.ENodeState> _onUpdate = null;

        public ActionNode(Func<IBTNode.ENodeState> onUpdate)
        {
            _onUpdate = onUpdate;
        }

        public IBTNode.ENodeState Evaluate()
        {
            return _onUpdate?.Invoke() ?? IBTNode.ENodeState.Failure;
        }
    }

    /*
     * Success나 Running 발생 시 그 노드까지 진행
     */
    public sealed class SelectorNode : IBTNode
    {
        List<IBTNode> _childs;

        public SelectorNode(List<IBTNode> childs)
        {
            _childs = childs;
        }

        public IBTNode.ENodeState Evaluate()
        {
            if (_childs == null)
                return IBTNode.ENodeState.Failure;

            foreach (var child in _childs)
            {
                switch (child.Evaluate())
                {
                    case IBTNode.ENodeState.Running:
                        return IBTNode.ENodeState.Running;
                    case IBTNode.ENodeState.Success:
                        return IBTNode.ENodeState.Success;
                }
            }

            return IBTNode.ENodeState.Failure;
        }
    }

    /***
     * Failure 발생할 때까지 진행
     * 자식 상태 : Running => Running 반환
     * 자식 상태 : Success => 다음 자식으로
     * 자식 상태 : Failure => Failure 반환
     */
    public sealed class SequenceNode : IBTNode
    {
        List<IBTNode> _childs;

        public SequenceNode(List<IBTNode> childs)
        {
            _childs = childs;
        }

        public IBTNode.ENodeState Evaluate()
        {
            if (_childs == null || _childs.Count == 0)
                return IBTNode.ENodeState.Failure;

            foreach (var child in _childs)
            {
                switch (child.Evaluate())
                {
                    case IBTNode.ENodeState.Running:
                        return IBTNode.ENodeState.Running;
                    case IBTNode.ENodeState.Success:
                        continue;
                    case IBTNode.ENodeState.Failure:
                        return IBTNode.ENodeState.Failure;
                }
            }

            return IBTNode.ENodeState.Success;
        }
    }

}


