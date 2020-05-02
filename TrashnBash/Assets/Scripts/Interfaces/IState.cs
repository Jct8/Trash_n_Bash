using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public interface IState
{
    void Enter();
    void Execute();
    void Exit();
}