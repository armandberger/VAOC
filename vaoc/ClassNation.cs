using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WaocLib;

namespace vaoc
{
    partial class Donnees
    {
        partial class TAB_NATIONDataTable
        {
            /// <summary>
            /// recherche le leader de la nation
            /// </summary>
            /// <returns>renvoie le pion correspondant</returns>
            public TAB_PIONRow CommandantEnChef(int idNation)
            {
                //le leader, c'est celui qui est propriétaire de lui-même
                //recherche du pion
                var result = from pionLeader in
                                 (from lignePion in Donnees.m_donnees.TAB_PION
                                  join ModelePion in Donnees.m_donnees.TAB_MODELE_PION
                                  on lignePion.ID_MODELE_PION equals ModelePion.ID_MODELE_PION
                                  join nation in Donnees.m_donnees.TAB_NATION
                                  on ModelePion.ID_NATION equals idNation
                                  select lignePion)
                             where pionLeader.ID_PION == pionLeader.ID_PION_PROPRIETAIRE
                             select pionLeader;

                if (0 == result.Count())
                {
                    return null;
                }

                return result.ElementAt(0);
            }
        }

        partial class TAB_NATIONRow
        {
            /// <summary>
            /// Nom du pion pour afficher dans une liste (sur FormPrincipale, la combo, et la liste des noms villes)
            /// </summary>
            public string nomListe
            {
                get { return string.Format("{0}:{1}", ID_NATION, S_NOM); }
            }

            /// <summary>
            /// recherche le leader de la nation
            /// </summary>
            /// <returns>renvoie le pion correspondant</returns>
            public TAB_PIONRow commandantEnChef
            {
                get
                {
                    //le leader, c'est celui qui est propriétaire de lui-même
                    //recherche du pion
                    var result = from pionLeader in
                                     (from lignePion in Donnees.m_donnees.TAB_PION
                                      join ModelePion in Donnees.m_donnees.TAB_MODELE_PION
                                      on lignePion.ID_MODELE_PION equals ModelePion.ID_MODELE_PION
                                      join nation in Donnees.m_donnees.TAB_NATION
                                      on ModelePion.ID_NATION equals this.ID_NATION
                                      select lignePion)
                                 where pionLeader.ID_PION == pionLeader.ID_PION_PROPRIETAIRE
                                 select pionLeader;

                    if (0 == result.Count())
                    {
                        return null;
                    }

                    return result.ElementAt(0);
                }
            }
        }
    }
}
