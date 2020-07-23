using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface ICharacterSound
{
    IEnumerator BasicSound(int id);
    IEnumerator BarricadeSound(int id);
}