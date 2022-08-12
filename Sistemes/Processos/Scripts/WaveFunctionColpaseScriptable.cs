using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Processos/WFC")]
public class WaveFunctionColpaseScriptable : ScriptableObject
{
    //1.-Triar tots els tiles que formaran part del WFC.
    //Estaria be que nom�s fossin els del voltant de la pe�a.
    //Osigui, els veins exteriors i els veins del veins.
    //Pero aix� pot portar a "colisions". [...]
    //Per tant, cambe agregarem els tiles de la pe�a per� amb una sola possiblitat.

    //2.- Propagacio:
    //Quan es selecciona un tile (en aquest cas ser�n els tiles de la pe�a colocada).
    //S'han de "actualitzar" les possiblitats de tots els veins.
    //Si aquestes possiblitats canvien (osgui, que no son com al inici de la comprovacio),
    //Es "propaga" aquesta actualitzacio a tots els seus veins.
    //[veus? crec que aquesta era la part que em deixava]

    //3.- Lowest entropy:
    //Busca el tile amb el minim de possibiliats.
    //Si nomes te una solcio, la seleccionar, si en te mes d'una tries en random.



    //Apunt!
    //nom�s el falta agregar la propagaci�, i els tiles inicials. que de fet, ja torna
    //ambiguus els tiles del voltant de la pe�a, nom�s faltaria que els de la pe�a fosin "fixos".
}
