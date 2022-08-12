using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Processos/WFC")]
public class WaveFunctionColpaseScriptable : ScriptableObject
{
    //1.-Triar tots els tiles que formaran part del WFC.
    //Estaria be que només fossin els del voltant de la peça.
    //Osigui, els veins exteriors i els veins del veins.
    //Pero això pot portar a "colisions". [...]
    //Per tant, cambe agregarem els tiles de la peça peró amb una sola possiblitat.

    //2.- Propagacio:
    //Quan es selecciona un tile (en aquest cas seràn els tiles de la peça colocada).
    //S'han de "actualitzar" les possiblitats de tots els veins.
    //Si aquestes possiblitats canvien (osgui, que no son com al inici de la comprovacio),
    //Es "propaga" aquesta actualitzacio a tots els seus veins.
    //[veus? crec que aquesta era la part que em deixava]

    //3.- Lowest entropy:
    //Busca el tile amb el minim de possibiliats.
    //Si nomes te una solcio, la seleccionar, si en te mes d'una tries en random.



    //Apunt!
    //només el falta agregar la propagació, i els tiles inicials. que de fet, ja torna
    //ambiguus els tiles del voltant de la peça, només faltaria que els de la peça fosin "fixos".
}
