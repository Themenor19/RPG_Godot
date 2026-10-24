using System.Collections.Generic;
using Godot;
namespace RPG.scripts.helper_classes;

public class LevelQueue
{
    private List<LevelQueueNode> _levelQueue = [];

    public void QueueLevel(LevelQueueNode levelQueueNode)
    {
        _levelQueue.Add(levelQueueNode);
    }
    public LevelQueueNode Pop()
    {
        var levelQueueNode = _levelQueue[0];
        _levelQueue.RemoveAt(0);
        return levelQueueNode;
    }
}

public class LevelQueueNode
{
    public string LevelUid { get; set; }
    public Vector2 Position { get; set; }
}