using System;

public class QuestionNode : ITreeNode
{
    private Func<bool> _question;
    private ITreeNode _fNode;
    private ITreeNode _tNode;

    public QuestionNode(Func<bool> question, ITreeNode tNode, ITreeNode fNode)
    {
        _question = question;
        _tNode = tNode;
        _fNode = fNode;
    }

    public void Execute()
    {
        if (_question()) _tNode.Execute();
        else _fNode.Execute();
    }
}
