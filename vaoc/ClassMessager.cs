﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.Threading;
using WaocLib;

namespace vaoc
{
    /// <summary>
    /// Classe de données utilisées pour rechercher les Pions environnants
    /// </summary>
    class Barycentre
    {
        public int x;
        public int y;
        public int nb;

        public Barycentre()
        {
            x=y=nb=0;
        }
    }

    public class ClassMessager
    {     
        #region constantes
        public enum COMPAS { CST_NORD = 0, CST_NORD_OUEST = 1, CST_OUEST = 2 , CST_SUD_OUEST = 3, CST_SUD = 4, CST_SUD_EST = 5, CST_EST = 6, CST_NORD_EST = 7 , CST_INDETERMINE = 8};
        private static readonly string[] lieuxMasculins= { "pont","carrefour", "chemin","fleuve" };
        private static readonly char[] voyelles = { 'a','e','i','o','u','y','î','ï','é','è','ê','ù','à' };
        private const double degre45 = 0.70710678118654752440084436210485;
        private static readonly string[] sequenceceFeminin = { "", "seconde", "troisième", "quatrième", "cinquième", "sixième", "septième", "huitième", "neuvième", "dixième", 
                                                        "onzième", "douzième", "treizième", "quatorzième", "quinzième", "seizième", "dixseptième", "dixhuitième", "dixneuvième", "vingtième" };
        private static readonly string[] sequenceceMasculin = { "", "second", "troisième", "quatrième", "cinquième", "sixième", "septième", "huitième", "neuvième", "dixième", 
                                                        "onzième", "douzième", "treizième", "quatorzième", "quinzième", "seizième", "dixseptième", "dixhuitième", "dixneuvième", "vingtième" };

        public const int CST_MESSAGE_FREQUENCE_ALERTE = 6;//nombre de tours entre deux messages d'alerte

        public enum MESSAGES
        {
            MESSAGE_PERSONNEL = 0,
            //Message d'un joueur vers un autre jour
            MESSAGE_FUITE_AU_COMBAT = 1,
            //ex : Mon général j'ai le regret de vous annoncer que le {0} la division {1} s'est enfuit de {2} démoralisée par le feu ennemi.
            MESSAGE_ARRIVE_A_DESTINATION = 2,
            //ex : A {0} la division {1} est arrivée au lieu dit {2}.
            MESSAGE_FUITE_ET_DETRUITE_AU_COMBAT = 3,
            //ex : Dans la bataille {3}, alors que j'essaierai de faire retraite avec l'unité {1} , nous avons été forcés de déposer les armes.
            MESSAGE_PERTES_AU_COMBAT = 4,
            //ex : Dans la bataille {1}, les forces de ma division {1} faiblissent. Notre moral a chuté de {10} mais nous sommes encore à {11} et continuons le combat.
            MESSAGE_AUCUNE_POURSUITE_POSSIBLE = 5,
            //émis par des unités qui ne peuvent pas poursuivre l'ennemi après un combat
            //ex : Nous avons vaincus l'ennemi à la bataille {3} mais nos forces ne nous permettent pas de poursuivre l'ennemi.
            //ex : Nous sommes victorieux ! La bataille {3} est terminée et nous restons maîtres du champ de bataille. L'état de nos forces ne nous permet cependant pas de poursuivre l'ennemi et de lui enfoncer l'épée dans les reins comme j'aurai pris grand plaisir à le faire.
            //ex : {16}, c'est par un temps {20} que nous avons vaincu l'ennemi. Cette bataille restera à jamais gravée dans les livres d'Histoire. Mon seul regret sera que nous n'avons pas été en mesure de poursuivre l'ennemi comme il aurait sans doute le faire.
            MESSAGE_AUCUNE_POURSUITE_RECUE = 6,//émis par une unité n'ayant pas été poursuivie après un combat
            //ex : En tant que commandant de la division {1}, je vous informe que la bataille {3} est terminée. Nous retraitons en bon ordre et l'ennemi renonce à nous poursuivre.
            MESSAGE_POURSUITE_SANS_EFFET = 7,//émis par les unités ayant conduit une poursuite sans effet
            //ex : la bataille {3} est terminée. La division {1} a été poursuivi à {0} par l'ennemi mais devant notre obstination, ils sont repartis sans tirer.
            MESSAGE_DETRUIT_PAR_POURSUITE = 8,//émis par une unité détruit par une poursuite
            //ex : Suite à la bataille {3}, l'ennemi nous a poursuivie, les dernières troupes de la division {1} ont été anéanties.
            MESSAGE_POURSUITE_DESTRUCTION_TOTALE = 9,//émis par les troupes ayant détruit tout l'ennemi en poursuite
            //ex : L'ennemi n'est plus ! Grace à notre remarquable poursuite, il vient de perdre encore {4} fantassins, {6} cavaliers et {8} artilleurs, à la bataille {3}.
            MESSAGE_POURSUITE_PERTES_POURSUIVANT = 10,//résultat des pertes provoqués par le poursuivant
            //ex : Nous avons victorieusement poursuivi l'ennemi après la bataille {3}. Sous nos balles et nos sabres, l'ennemi a encore perdu {4} fantassins, {6} cavaliers et {8} artilleurs supplémentaires.
            MESSAGE_POURSUITE_PERTES_POURSUIVI = 11,
            //résultat des pertes provoqués par le poursuivant
            //ex : Durant notre retraite suivant la bataille {3}, l'ennemi est tombé sur nous, nous infligeant encore la perte de {4} fantassins, {6} cavaliers et {8} artilleurs à notre division {1} avant qu'ils ne s'enfuient.
            MESSAGE_GAIN_MORAL_MAITRE_TERRAIN = 12,//gain au moral pour les unité présentent sur le champ de bataille
            //ex : Galvanisés par notre victoire à la bataille {3}, le moral de la division {1} est remonté de {10} pour un total de {11}
            MESSAGE_FIN_DE_BATAILLE_A_LA_NUIT = 13,
            //ex : A la tombée de la nuit la bataille {3} s'est arrêtée. Notre division {1} est prête à reprendre dés demain.
            MESSAGE_BILAN_REPOS_MORAL_ET_FATIGUE = 14,//l'unité s'est reposée et à regagner du moral et de la fatigue
            //ex : Les hommes de la division {1} se sont bien reposés aujourd'hui, notre moral est remonté de {10} pour un total de {11} et un maximum de {12} et notre fatigue de {13} pour un total de {14}
            //ex : La {1} a pû prendre ses quartiers durant toute la journée, nous n'avons plus que {14} % des effectifs manquants et le moral remonte ! Les hommes sont de nouveau prêts à passer à l'action.
            MESSAGE_BILAN_REPOS_MORAL = 15,//l'unité s'est reposée et à regagner du moral
            //ex : Les hommes de la division {1} ont pris du repos aujourd'hui, j'en ai profité pour regonfler le moral des troupes de {10} pour un total de {11} sur un maximum de {12}.
            MESSAGE_BILAN_REPOS_FATIGUE = 16,//l'unité s'est reposée et à regagner de la fatigue
            //ex : Après de longues marches, la division {1} a bivouaqué aujourd'hui, diminuant la fatigue de {13}, nous sommes donc maintenant reposés à {14} %.
            MESSAGE_BILAN_REPOS = 17,//l'unité s'est reposée et n'a rien regagnée
            //ex : Tous les hommes de la division {1} sont frais et dispos maréchal. Notre moral est au plus haut, j'attends vos Constantes.ORDRES.
            MESSAGE_BILAN_ACTION = 18,//l'unité a agi durant cette journée
            //ex : Tous les hommes de la division {1} ont bien agis aujourd'hui conformément à vos Constantes.ORDRES. Nous sommes fatigués à {14} %.
            //ex : Une nouvelle journée se termine. J'ai mis tout en œuvre pour suivre vos demandes. Ayant toujours à cœur de vous satisfaire, n'hésitez pas à me solliciter.
            //ex : Les hommes de troupe de {1} sont fatigués à {14} %. Je bivouaque actuellement à {2} prêt à me rendre à me rendre à ma destination finale. Je reste votre dévoué serviteur.
            MESSAGE_DETRUITE_AU_COMBAT = 19,
            //ex : Malgré tout notre courage, mes derniers hommes viennent de tomber sous le feu de l'ennemi, à {0} la division {1} n'existe plus citoyen.
            MESSAGE_AUCUN_MESSAGE = 20,
            //ex : Aucune aide de camp ne s'est présenté à votre quartier.
            MESSAGE_PERTES_TOTALE_AU_COMBAT = 21,
            //ex : A la fin de {3}, nos pertes s'élèvent à {4} fantassins, {6} cavaliers, {8} artilleurs.
            MESSAGE_FIN_DE_BATAILLE_FAUTE_DE_COMBATTANT = 22,
            //ex : {3} se termine car toutes les troupes ont quittées le lieux des combats, nos pertes s'élèvent à {4} fantassins, {6} cavaliers, {8} artilleurs.
            MESSAGE_DEPART_VERS_DESTINATION = 23,
            //ex : Conformément aux ordres que je viens de recevoir les hommes de {1} se mettent en route vers {2}.
            MESSAGE_PATROUILLE_CONTACT_ENNEMI = 24,
            //ex : Une patrouille est revenue, alors qu'elle se rendait à son point de destination, elle a trouvé les forces ennemies suivantes : {21} et est rentrée immédiatement faire son rapport.
            MESSAGE_PATROUILLE_RAPPORT = 25,
            //ex : Une patrouille revient, l'officier commandant la patrouille affirme avoir observer les unités suivantes : {21}.
            MESSAGE_CHEMIN_BLOQUE = 26,
            //ex : Mon unité ne peut plus avancer, elle est bloquée par d'autres unités à {2}.
            MESSAGE_ORDRE_MOUVEMENT_RECU = 27,
            //ex : J'ai bien reçu votre ordre de {22} pour aller à {23} de {24}h00 à {25}h00. Je l'applique immédiatement.
            MESSAGE_RENFORT = 28,
            //ex : L'unité {1} vient de se mettre sous vos ordes. Les hommes sont actuellement positionnés en {2}. Je suis prêt à executer vos Constantes.ORDRES.
            MESSAGE_ORDRE_MOUVEMENT_IMPOSSIBLE_AU_COMBAT = 29,
            //ex : J'ai bien reçu votre ordre de mouvement depuis {2} mais étant engagé au combat, je ne peux en tenir compte. Sachez bien que je le regrette.
            //ex : Mon unité est actuellement engagée dans {3}. Je ne peux donc executer votre ordre de mouvement.
            MESSAGE_ORDRE_PATROUILLE_RECU = 30,
            //ex : J'ai bien reçu votre ordre d'envoie de patrouille en {23}. Celle-ci vient de se mettre en route.
            MESSAGE_BRUIT_DU_CANON = 31,
            //ex : {1} signale le bruit d'une bataille à {26} alors qu'elle se trouve à {2}.
            //ex : Le son d'un canon résonne aux oreilles de {1}. Le son semble provenir de {26} sachant que sa position actuelle est {2}.
            MESSAGE_ARRIVEE_DANS_BATAILLE = 32,
            //ex : Je me retrouve dans la zone de combat de la {3}. Mon unité, la glorieuse et fidèle {1} attend des consignes.
            MESSAGE_ENGAGEMENT_AUTOMATIQUE_DANS_BATAILLE = 33,
            //ex : L'ennemi avance dans la {3}. J'ai donc pris sur moi d'engager mes troupes pour arrêter l'ennemi.
            MESSAGE_VICTOIRE_EN_BATAILLE = 34,
            //ex : Nous sommes victorieux dans la {3}. Nos troupes occupent le terrain. Nous sommes prêts à recevoir de nouveaux ordres pour continuer vers la victoire.
            //ex : Victoire ! A {3} l'ennemi est défait et nous restons seuls maîtres du terrain. C'est un grand jour pour notre armée et notre pays.
            MESSAGE_DEFAITE_EN_BATAILLE = 35,
            //ex : {3} est terminée. Malgré tout notre courage, nous abandonnons le terrain à l'ennemi. Nous attendons les Constantes.ORDRES.
            MESSAGE_INTERCEPTION = 36,
            //ex : A {0} nos troupes ont capturés un messager égaré,il était porteur du message suivant : {27}.
            MESSAGE_TIR_SUR_ENNEMI = 37,
            //ex : A {3} le tir conjugués de nos troupes sur zone font perdre {10} au moral de l'ennemi.
            //ex : Dans la fureur de la bataille, à {0}, l'ennemi perd {10} de moral sous l'effet de notre action sur zone.
            //ex : Le combat fait rage mais nos troupes unies ont fait perdre un moral de {10} à toutes les troupes ennemies qui nous font face.
            MESSAGE_EST_DETRUIT_AU_CONTACT = 38,
            //l'unité a détruit une autre unité en entrant à son contact
            //ex : J'ai le plaisir de vous annoncer une excellente nouvelle, nous avons surpris et détruit une unité ennemi : {29}. Une belle opération réalisée à peu de prix grace à vos ordres judicieux.
            MESSAGE_A_DETRUIT_AU_CONTACT = 39,
            //unite detruire au contact de l'ennemi
            //ex : {17}, c'est une catastrophe ! Notre unité : {1} a été surprise par l'ennemi, les hommes du {29} ne disposant d'aucune force defensive, nous avons dû nous rendre immédiatement.
            MESSAGE_SANS_RAVITAILLEMENT = 40,
            //l'unite n'est plus ravitaillée
            //ex : {0} : mon unité n'est plus actuellement ravitaillée, j'espère qu'il ne s'agit que d'un état temporaire.
            //ex : {0} : Je ne suis plus en contact avec l'un de nos dépôts, mes troupes n'ont donc pas pû se ravitailler et reprendre des forces comme je l'escomptais.
            MESSAGE_NOUVEAU_RAVITAILLEMENT = 41,
            //l'unité vient de retrouver un ravitaillement
            // ex: Très bonne nouvelle, je suis dorénavant ravitaillé par le {31} situé à une distance de {30} kilomètres de notre position. Nos besaces se remplissent à nouveau.
            MESSAGE_REDITION_POUR_RAVITAILLEMENT = 42,
            //l'unité se rend par manque de ravitaillement -> non utilisé pour l'instant, l'unité n'est simplement pas combattive comme avez zéro de moral
            //ex : Affamés, sans munition et desespérés, mon unité s'est rendu en proie à une désagrégation complète. A {0}, la {1} n'est plus.
            MESSAGE_EST_CAPTURE = 43,
            //l'unité a été capturé par une autre (cas d'un dépôt au contact avec une unité ennemie).
            //ex : {17}, c'est une catastrophe ! Notre unité : {1} a été surprise par l'ennemi, les hommes du {29} ont été capturés, nous voilà prisonniers.
            MESSAGE_A_CAPTURE = 44,
            //l'unité vient de capturer une autre unité, probablement un dépôt.
            // ex: Coup de chance, nous venons de capturé une unité ennemi :  {29}. Ceux-ci ce sont immédiatement rendus en nous voyant, sans même avoir à tirer, les lâches !
            MESSAGE_DESTINATAIRE_INTROUVABLE = 45,
            //ex : {17}, arrivé à {2}, je n'ai pas trouvé {29} à qui vous m'aviez donné l'ordre de porter un message. Je reviens donc sans avoir pû remplir ma mission.
            //ex : Il est {0} et parvenu à {2}, je n'ai pas trouvé le haut personnage auquel vous m'aviez demandé de porter unne missive. J'ai intérrogé un marchant de passage mais il ne savait pas non plus où se trouvait cet homme. Me voilà donc bredouille.
            //ex : Vous m'aviez ordonné de porter un message à {23} mais je n'ai pas trouvé le destinataire que vous m'aviez indiqué à cet emplacement. Je reviens donc vous voir pour vous demander si vous savez quel est la nouvelle position géographique du destinataire.
            MESSAGE_POSITION = 46,
            //Message indiquant simplement sa position pour une autre unité
            //ex : La météo est {20} actuellement et je suis actuellement en position à {2}.
            //ex: Je suis actuellement positionné à {2}. J'attends de vos nouvelles.
            //ex: Moi, {1}, je me trouve en ce moment même à {2}. N'hésitez pas à me contacter si vous en avez besoin. 
            //ex: A l'instant où je vous écris, mon unité est à {2}. Sachez que je ne doute point de notre victoire, {16}.
            //ex: A {0} je me trouvais à {2}. Si vous avez besoin d'informations vous savez maintenant où me trouver.
            MESSAGE_DESTINATION_IMPOSSIBLE = 47,
            //Message indiquant qu'il n'est pas possible de se rendre vers le lieu indiqué
            //ex : A {0} je reçois l'ordre de me rendre à {23}. J'ai longuement consulté mes cartes mais je ne vois aucune route me permettant de me rendre sur ce lieu. Je me vois donc au regret de ne pouvoir suivre vos consignes.
            //ex : {17}, je viens de recevoir votre message m'ordonnant de me rendre à {23}. Cependant, je ne vois aucun moyen de me rendre vers ce lieu, auriez vous une autre route à m'indiquer afin de suivre vos directives ?
            MESSAGE_TRAVERSE_GUE = 48,
            //Message indiquant que l'on traverse un "gué", il peut s'agir d'un "vrai" gué ou d'un pont détruit
            //ex : Je traverse un {32} à {0} situé en {1}. Cela n'était pas préu lorsque j'ai planifié mon mouvement et devrait donc ralentir mon arrivée à la destination prévue.
            //ex : {17}, pour votre information, je suis face à un {32}. Cet obstacle n'est pas insurmontable va engendrer du retard dans ma progression. Ne vous étonnez donc pas si je n'arrive pas exactement à l'heure prévue.
            MESSAGE_CHEMIN_IMPRATICABLE = 49,
            //Message lorque l'unité arrive sur une case impossible à traverser, cela peut arriver si l'unité à prévu de prendre un ponton qui a été détruit
            //ex : Alors que je me rends à {23}, conformément à vos ordres, je fais face à un obstacle imprévu à {2} et infranchissable. Je vais donc prendre un autre chemin pour parvenir à destination.
            //ex : Je suis arrivé à {0} à {2} mais le chemin est coupé et est devenu impraticable. Je vais donc dévier de ma route pour me rendre là où vous me l'avez ordonné.
            MESSAGE_PONT_INTROUVABLE = 50,
            //Message indiquant qu'il n'y a aucun pont à proximité qui puisse être détruit ou réparé
            //ex: {17} Vous m'avez donner l'ordre de {21} mais je ne vois aucun pont à proximité du lieu où je me trouve actuellement. Je ne peux donc pas executer votre ordre malgré toute la bonne volonté et le respect qui m'animent.
            //ex: Je suis actullement à {2} et, aussi loin que porte mon regard, je ne vois aucun pont dans les parages. Je ne comprends donc pas comment je pourrai executer votre ordre de {21} et croyez bien que je le regrette.
            MESSAGE_PONT_DETRUIT = 51,
            //Message informant la destruction d'un pont
            //ex: C'est dans une grande explosion que le tablier central du pont situé à {23} vient de s'effondrer. Nos artilleurs ont fait là un remarquable travail. Voilà qui devrait considérablement retarder la progression de l'ennemi.
            //ex: le grand feu que nous avons allumé sur le pont de bois situé à {23} vient de terminer de se consummer. Vu l'état noirci des planches, je doute que l'ennemi ose s'y aventurer sans prendre de grandes précautions ! 
            MESSAGE_PONT_REPARE = 52,
            //Message informant la réparation d'un pont
            //ex: Les hommes rangent les derniers outils et s'épongent le front, car le tablier central du pont situé à {23} vient d'être réparé. Voilà un joli travail, nous troupes vont à nouveau pouvoir passer la rivière sans perdre de temps.
            //ex: Enfin le pont que vous nous avez donné l'ordre de réparer à {23} est réparé. Informez tous les officiers que les troupes peuvnet à nouveau l'emprunter. 
            MESSAGE_PONTON_CONSTRUIT = 53,
            //Message informant la construction d'un ponton
            //ex: A {0} les derniers clous terminant le pont de bateaux que nous érigeons à {23} viennent d'être frappés. Les troupes peuvent maintenant y marcher sans mouilleur leurs souliers.
            //ex: Je viens de traverser le ponton à {23} que vous m'avez donner l'ordre de construire. C'est un bel ouvrage conforme aux normes du génie et de l'armée. Je le déclare apte au service. Les troupes peuvent dés maintenant s'y engager sans risques.
            MESSAGE_ORDRE_REPORTE_ENNEMI_PROCHE = 54,
            //Message indiquant qu'un ordre (construction de ponton, réparation de pont) est reporté à cause d'un ennemi à proximité
            //ex: L'ordre de {21} que vous m'avez transmis vient d'être interrompu précipitament car des troupes ennemies sont à proximité. L'ordre sera suivi dés qu'elles seront parties.
            //ex: Mon unité est actuellement entrain de {21} sur un ouvrage situé en  {23} mais des troupes ennemies fort menançants nous obligent à suspendre le travail. Pourriez vous envoyez des troupes pour les mettre hors d'état de nuire ?
            MESSAGE_CHEMIN_BLOQUE_PAR_ENNEMI_LA_NUIT = 55,
            //Message indiquant un mouvement bloqué par un ennemi mais sans combat car il fait nuit
            //ex: {17}, tandis que je manoeuvrai de nuit, mon mouvement a été arrété par des troupes ennemies. Si vous connaissez un moyen de les contourner, veillez m'indiquer la route à suivre sans tarder.
            //ex: Alors que j'effectuais mon mouvement nocturne en toute discrétion, j'ai rencontré à {0} des forces ennemies à {2} qui m'empèchent d'avancer. Sans nouvel ordre de votre part, ou si l'ennemi ne quitte pas la zone, je me vois obligé de rester sur ma position actuelle.
            MESSAGE_ORDRE_RECU = 56,
            //message indiquant qu'une unité à bien reçu son ordre
            //ex: C'est à {0} que je prends connaissance de l'ordre de {21} qu'un messager vient de me remettre. J'en prends bonne note.
            //ex: Alors que je me trouvais en {23}, l'un de vos messagers vient de m'apporter un ordre de {21}. Je vais maintenant m'appliquer à le réaliser de façon adéquate.
            MESSAGE_ENNEMI_OBSERVE = 57,
            //Message envoyé quand un ennemi vient d'entrer dans le champ d'observation
            //ex: {17}, je vous envoie avec empressement un messager car je viens d'apercevoir l'ennemi depuis mon point d'observation, voilà ce que j'observe à {0} : {22}.
            MESSAGE_SANS_ENNEMI_OBSERVE = 58,
            //Message envoyé quand un ennemi vient de quitter le champ d'observation
            //ex: Je viens de refaire un tour d'horizon depuis mon observatoire à {2}. Je n'ai plus aucun ennemi en vue. Celui-ci a disparu.
            //ex: L'ennemi se dérobe à ma vue, de ma position, je vois maintenant les éléments suivants : {22}.
            MESSAGE_BRUIT_ENDOMMAGE_PONT = 59,
            //ex : {1} signale le bruit d'une forte explosion à {26} alors qu'elle se trouve à {2}.
            //ex : Une colonne de fumée s'élève dans le ciel qui semble provenir de {26} sachant que ma position actuelle est {2}.
            MESSAGE_BRUIT_REPARER_PONT_PONTON = 60,
            //ex : {1} signale le bruit de menuiserie à {26} alors qu'elle se trouve à {2}.
            //ex : Le son de scies, marteaux et hache résonne aux oreilles de {1}. Le son semble provenir de {26} sachant que ma position actuelle est {2}.
            MESSAGE_RECU_RAVITAILLEMENT = 61,
            // ex: La {1} a été ravitaillée le {31} situé à une distance de {30} kilomètres de notre position. Le matériel a progréssé de {37} % pour un total de {35} % tandis que le ravitaillement est maintenant à {34} % avec une progression de {36} %.
            MESSAGE_DEPOT_REDUIT = 62,
            // ex: Le {1} vient d'être réduit car nos stocks ont été utilisés pour remettre à niveau un très grand nombre de soldats. Envoyer nous un convoi, si possible, pour remonter nos stocks.
            MESSAGE_DEPOT_DETRUIT = 63,
            // ex: Le {1} n'existe plus, en effet, tous nos stocks ont été pris par des soldats de passage.
            MESSAGE_ORDRE_SUIVANT = 64,
            //ex: A {0}, je poursuis la feuille de route que vous m'aviez confié et j'entame l'execution de l'ordre {22} que vous avez demandé.
            //ex: Mon unité, la fidèle {1} vient de terminer sa tâche en cours et poursuit vos consignes en commençant de prendre en compte l'ordre {22} qui le suit.
            MESSAGE_A_PERDU_TRANSFERT = 65,
            //l'unité a été transféré à un autre officer (ou capture).
            //ex : {17}, La {1} a été transféré sous un autre commandement.
            //ex : {17}, La {1} a été n'est plus sous vos ordres.
            MESSAGE_A_RECU_TRANSFERT = 66,
            //l'unité vient d'avoir une nouvelle unité qui lui ait transféré.
            // ex: La {1} vient de passer sous votre autorité.
            // ex: La {1} est dorénavant sous vos ordres.
            MESSAGE_BLESSES_ARRIVE_A_DESTINATION = 67,
            // ex: La {1} est arrivé à l'hopital situé à {2}. Les médecins ont immédiatement pris en charge les cas les plus graves, les autres devraient rapidement se rétablir pour reprendre le combat.
            MESSAGE_PRISONNIERS_ARRIVE_A_DESTINATION = 68,
            // ex: Les prisonniers de la {1} sont arrivés à {2}. Ils ont été mis en prison où ils devront atteindre la fin du conflit. L'escorte est à vos ordres pour revenir s'intégrer à son unité.
            // ex: J'ai délivré les prisonniers qui étaient sous ma charge à la prison de {2}. Mon unité a donc terminé son rôle d'escorte et peut revenir à des taches plus combattives.
            MESSAGE_BLESSES_SOIGNES = 69,
            // ex: L'hopital de {2} est parvenu à remettre de nombreux hommes actifs. Ceux-ci sont prêts à rejoindre l'unité que vous voudrez bien leur assigner.
            MESSAGE_PRISONNIER_LIBERE = 70,
            // ex: La prison de {2} a été libérée ! Les soldats qui s'y trouvaient retenus sont prêts à rejoindre leurs rangs et combattre à nouveau sous vos ordres.
            MESSAGE_GENERATION_CONVOI = 71,
            // ex: Le convoi de ravitaillement que vous avez ordonné est en place et prêt à se mettre en mouvement à votre demande.
            // ex: A {0}, un convoi est disponible et prêt à partir à {2}.
            MESSAGE_RENFORT_DEPOT = 72,
            // ex: Un convoi de ravitaillement vient de me parvenir. Notre dépôt est maintenant bien approvisionné.
            // ex: Un convoi de ravitaillemen vient d'approvisionner la {1} à {0}.
            MESSAGE_RENFORT_UNITE = 73,
            // ex: Des renforts viennent de me parvenir, nos effectifs sont en hausse et la fatigue de l'unité a évolué de {11}%.
            MESSAGE_FORTIFICATION_TERMINEE_UN = 74,
            // ex: Après de longs efforts, la {1} vient de terminer des fortifications de campagne. Nous continuons à améliorer notre position.
            MESSAGE_FORTIFICATION_TERMINEE_MAX = 75,
            // ex: Après de très longs efforts, la {1} vient de terminer des fortifications de campagne. Ce bel ouvrage ne peut plus être amélioré.
            // ex: L'ensemble de notre dispositif de défense est en place, la {1} est maintenant solidement retranchéses, prête à repousser l'ennemi.
            MESSAGE_POURSUITE_BUTIN = 76,
            // ex: Ah ! Ah ! Belles prises ! Sur les troupes poursuivies nous avons récupérés {37}% de matériel, {36}% de ravitaillement et {8} canons.
            MESSAGE_PRISONNIERS_APRES_BATAILLE = 77,
            // ex: Je viens de constituer la {1} à partir des prisonniers fait à la bataille. Ils sont prêt à être accompagnés sous bonne escorte jusqu'à la prison que vous jugerez le plus sure
            // ex: C'est sous bonne escorte que la {1} est prêt à conduire tous ses hommes perdus pour leur pays dans la prison la plus adéquate.
            MESSAGE_SOINS_APRES_BATAILLE = 78,
            // ex: En parcourant le lieu de {3} j'ai pu prodigué des soins à {33} et les réintégrer à l'unité.
            // ex: Après notre belle victoire à {3}, nos chirurgiens ont fait des merveilles sur les blessés et remis rapidement sur pied {4} fantassins et {6} cavaliers.
            MESSAGE_BLESSES_APRES_BATAILLE = 79,
            // ex : Je compte {33} blessés après {3} qui sont constitués en convoi. Leur état necessite des soins importants que seul un hopital peut fournir.
            // ex : La {1} regroupe tous les blessés de {3}. Le mieux serait de les diriger vers l'hopital le plus proche.
            MESSAGE_MALADES_RECUPERATION = 80,
            //sur la récupération des trainards, 2/10 sont malades et constituent des blessés
            // ex : La {1} regroupe tous les hommes malades de l'unité. Le mieux serait de les diriger vers l'hopital le plus proche pour les y faire soigner.
            MESSAGE_CHEF_REMPLACANT = 81,
            // ex : {1} prend officielement à cette heure le commandement du corps d'armée en remplacement de {38}. C'est avec tout le sens du devoir dû à mon rang que je prends en main la destinée de mes troupes.
            MESSAGE_RETOUR_CHEF = 82,
            // ex : Après ma blessure, me voilà parfaitement remis et je reprends le commandement de mon corps d'armée à partir du {0}.
            // ex : En arrivant à {2}, je suis ravi de voir que mes unités ont été parfaitement tenues durant ma convalescence. Je suis de nouveau prêt pour le service et à assumer ma charge.
            MESSAGE_ETABLIRDEPOT = 83,
            // ex : {1} vient d'être établi sur site. Nous allons maintenant pouvoir ravitailler nos troupes.
            // ex : A {0}, j'ai fini d'établir notre dépôt, mes hommes vont pouvoir approvisionner notre belle armée.
            MESSAGE_CHEF_BLESSE = 84,
            // ex : Ayant reçus plusieurs blessures, mon état de santé ne me permet plus de tenir mon rang. A {0} je quitte mon poste pour plusieurs jours.
            MESSAGE_CHEF_TUE = 85,
            //ex : Je suis à l'agonie, je meurs...
            //ex : En tant qu'aide de camp du {1}, je dois vous annoncer avec une profonde tristesse que celui vient de mourrir à mes pieds, frappé en plein coeur par une balle ennemie.
            MESSAGE_RAPPORT_PRISON = 86,
            //En tant que directeur de la prison de {2}, les effectifs des goeles sont de {5} soldats d'infanterie et {7} soldats de cavalerie. 
            MESSAGE_RAPPORT_HOPITAL = 87,
            //En tant que médecin général responsable de l'hopital de {2}, j'informe que je soigne actuellement dans mes services {5} fantassins et {7} cavaliers.
            MESSAGE_CONTACT_PATROUILLE_ENNEMIE = 88,
            //Une patrouille ennemie a été observé autour de la {1}. Elle est rapidement repartie sans que nous puissions intervenir.
            //Une patrouille ennemie est entrée en contact à {2}. Quelques tirs bien ajustés, l'on rapidement mise en fuite.
            //Quelques cavaliers, sans doute une patrouille, ont affrontés nos soldats à {2}. Dès le premier sang versés, ils se sont tous évanouis dans la nature...
            MESSAGE_CREATION_DEPOT_IMPOSSIBLE = 89,
            //Les bâtiments présents à {2} ne permettent pas d'y créer un dépôt de grande taille. Le convoi doit se rendre ailleurs ou constituer un autre dépôt.
            //On ne peut pas créer un dépôt de grande taille ici, le chef du convoi est dans l'incapacité de renforcer le dépôt présent
            //Le dépôt ne peut pas décharger à {2}, il n'y a pas assez de place.
            MESSAGE_RAVITAILLEMENT_DIRECT = 90,
            // La {1} a pu se rééquiper sur le {31}. Le matériel a progréssé de {37} % pour un total de {35} % tandis que le ravitaillement est maintenant à {34} % avec une progression de {36} %.
            // Le temps est {20} et nous avons pu nous ravitailler comme prévu sur le {31}.
            // Notre matériel est maintenant à {35} % et notre ravitaillement à {34} %  suite au prévelement effectué sur  le {31}.
            // Le {31} a pu nous fournir de  quoi remplir nos havresacs.
            // Nos besaces sont maintenant mieux remplies suite à notre passage dans les stocks du {31}.
            MESSAGE_RAVITAILLEMENT_DIRECT_IMPOSSIBLE = 91,
            //Nous n'avons pas pu nous ravitailler comme vous l'aviez ordonné, soit aucun dépôt n'était présent soit il ne pouvait pas nous ravitailler.
            //Aucun dépôt n'était disponible pour nous fournir le ravitaillement ou le matériel attendu.
            //Nous n'avons pas trouvé à notre emplacement un dépôt capable de nous donner quelque chose.
            MESSAGE_LIEU_POINT_DE_VICTOIRE = 92,
            //Notre position vient d'être capturée par l'ennemi.
            //{1} est passée sous le contrôle de l'ennemi, j'ai pris tous les risques pour vous faire parvenir ce message.
            //Des informations concordantes de différents observateurs indique que {1} n'est plus sous notre autorité.
            MESSAGE_RENFORT_IMPOSSIBLE = 93,
            //Arrivé à destination je recherche, sans succès, l'unité à renforcer sur place mais celle-ci reste invisible. Que dois-je faire maintenant ?
            //J'ai bien executé votre ordre demandant de renforcer les éléments présents mais les éléments n'étant en définitive pas présent, je ne peux rien faire. J'attends de nouvelles consignes.
            //{1} est prêt à renforcer l'unité en suivant vos ordre mais celle-ci n'est pas présente à {2} et je ne peux donc rien faire de plus. 
            MESSAGE_RENFORT_DEPOT_A_IMPOSSIBLE = 94,
            //Le dépôt de destination ne peut pas être renforcé, mon convoi reste donc sans affectation
            //{1} est arrivé à {2} mais le dépôt que je dois renforcer n'est pas en mesure d'accepter mes chariots. Je suis en attente de nouvelles consignes. 
            MESSAGE_POSITION_OFFICIERS = 95,
            //Renseignements pris auprès des divisions sur place, leurs officiers sont positionnés à {22}.
            //{1} vos aides de camp ont intérrogés les autres soldats présents, ils nout ont informés que leurs officiers sont à {22}
            //En interrogeant les soldats à {2} nous avons appris que leurs chefs sont à {22}
            MESSAGE_AUCUN_ENNEMI_PROCHE = 96,
            //A {0}, je ne trouve aucun ennemi à proximité de ma position, je ne peux donc passer à l'attaque comme vous me l'avez ordonnné.
            //Aussi loin que porte mon regard, je ne trouve aucun adversaire à affronter dans les environs. Votre ordre ne peut donc être suivi.
            //MESSAGE_ENNEMI_PROCHE_INATTAQUABLE = 97
            //J'ai bien tenté de passer à l'attaque à {2} mais d'autres unités m'empèchent d'engager le combat. Les unités suivantes sont actuellement observables : {21}.
            //Il y a bien une unité ennemie à coté de moi mais je ne peux la combattre car elle est protégée par d'autres forces. Voilà ce que j'observe depuis ma position : {21}
            MESSAGE_RAVITAILLEMENT_DIRECT_IMPOSSIBLE_SANS_REPOS = 97,
            //Nous n'avons pas pu nous ravitailler directemente sur un dépôt comme vous l'aviez ordonné car nous avons effectué d'autres activités ce jour. L'ordre est annulé.
            //Nous n'avons pu nous ravitailler directement sur un dépôt car nous vous nous aviez assigné d'autres tâches ce jour. L'ordre est annulé.
            //Pour effectuer un ravitaillement direct, nous devons y consacrer toute notre journée, ce qui ne fut pas le cas aujourd'hui. L'ordre est annulé.
            MESSAGE_RENFORT_TERMINE = 98,
            // Les renforts sont parvenus à destination. Cette unité est donc dissoute.
            // Nous avons renforcées l'unité souhaitée, cette formation n'a plus lieu d'exister.
            // Notre tâche est maintenant accompli et nous existons dorénavant à travers notre nouvelle unité.
        }
        /*
               DateHeure(true), //0
                lignePion.S_NOM, //1
                nomZoneGeographique,
                nomBataille, //3, sous la forme "la bataille de...
                iPertesInfanterie,
                lignePion.infanterie,//5
                iPertesCavalerie,
                lignePion.cavalerie,//7
                artilleriePerduOuGagne,
                lignePion.artillerie,
                moralPerduOuGagne,//10 
                lignePion.Moral,
                lignePion.I_MORAL_MAX,//12
                fatiguePerduOuGagne,
                lignePion.I_FATIGUE,//14
                iTourSansRavitaillement,
                criRalliement,//16
                nomDuSuperieur,
                unitesAttaquantes,//18
                nationsAttaquantes,
                Donnees.m_donnees.TAB_METEO[Donnees.m_donnees.TAB_PARTIE[0].ID_METEO].S_NOM,//20
                unitesEnvironnantes,
                ordreType,//22
                nomZoneGeographiqueOrdre,
                ordreHeureDebut,//24
                ordreDuree,
                distanceBataille,//26
                messageInterception ou message non remis,//27
                effectifs,//28
                nomCible ou personne à qui devait être remis le message,
                distanceRavitaillement, //30
                depotRavitaillement,
                ModeleTerrain, //32
                effectifsPerdus //33
                lignePion.I_RAVITAILLEMENT,
                lignePion.I_MATERIEL,//35
                ravitaillementGagneOuPerdu,
                materielGagneOuPerdu//37
                nomDuChefRemplace//38
                modele de l'unité (son nom donne une indication sur la nation) //39
*/
        #endregion

        public static COMPAS DirectionOrdreVersCompas(int i_direction)
        {
            COMPAS compas = i_direction switch
            {
                0 => ClassMessager.COMPAS.CST_NORD,
                1 => ClassMessager.COMPAS.CST_NORD_EST,
                2 => ClassMessager.COMPAS.CST_EST,
                3 => ClassMessager.COMPAS.CST_SUD_EST,
                4 => ClassMessager.COMPAS.CST_SUD,
                5 => ClassMessager.COMPAS.CST_SUD_OUEST,
                6 => ClassMessager.COMPAS.CST_OUEST,
                7 => ClassMessager.COMPAS.CST_NORD_OUEST,
                _ => ClassMessager.COMPAS.CST_INDETERMINE,
            };
            return compas;
        }

        public static string DirectionOrdreVersCompasString(COMPAS compas, bool avecLiaison)
        {
            string strDirection = compas switch
            {
                COMPAS.CST_NORD => (avecLiaison) ? "au Nord" : "le Nord",
                COMPAS.CST_NORD_OUEST => (avecLiaison) ? "au Nord-Ouest" : "le Nord-Ouest",
                COMPAS.CST_OUEST => (avecLiaison) ? "à l'Ouest" : "l'Ouest",
                COMPAS.CST_SUD_OUEST => (avecLiaison) ? "au Sud-Ouest" : "le Sud-Ouest",
                COMPAS.CST_SUD => (avecLiaison) ? "au Sud" : "le Sud",
                COMPAS.CST_SUD_EST => (avecLiaison) ? "au Sud-Est" : "le Sud-Est",
                COMPAS.CST_EST => (avecLiaison) ? "à l'Est" : "l'Est",
                COMPAS.CST_NORD_EST => (avecLiaison) ? "au Nord-Est" : "le Nord-Est",
                COMPAS.CST_INDETERMINE => "Indéterminé",
                _ => "Indéterminé",
            };
            return strDirection;
        }

        public static bool CaseVersCompas(int ID_CaseSource, int ID_CaseDestination, out COMPAS direction)
        {
            Donnees.TAB_CASERow ligneCaseSource = Donnees.m_donnees.TAB_CASE.FindParID_CASE(ID_CaseSource);
            Donnees.TAB_CASERow ligneCaseDestination = Donnees.m_donnees.TAB_CASE.FindParID_CASE(ID_CaseDestination);
            if (ligneCaseSource == null || ligneCaseDestination == null)
            {
                direction = COMPAS.CST_NORD;
                return false;
            }
            return CaseVersCompas(ligneCaseSource, ligneCaseDestination, out direction);
        }

        public static bool CaseVersCompas(Donnees.TAB_CASERow ligneCaseSource, Donnees.TAB_CASERow ligneCaseDestination, out COMPAS direction)
        {
            return CaseVersCompas(ligneCaseSource.I_X, ligneCaseSource.I_Y, ligneCaseDestination.I_X, ligneCaseDestination.I_Y, out direction);
        }

        public static bool CaseVersCompas(int CaseSourceX, int CaseSourceY, int CaseDestinationX, int CaseDestinationY, out COMPAS direction)
        {
            double angle;

            //recherche de la direction générale
            angle = Constantes.AngleOfView(CaseSourceX, CaseSourceY,
                                        CaseDestinationX, CaseDestinationY,
                                        CaseSourceX+100, CaseSourceY);
            if (CaseSourceY > CaseDestinationY)
            {
                if (angle < 22.5)
                {
                    direction = COMPAS.CST_EST;
                    return true;
                }
                if (angle < 67.5)
                {
                    direction = COMPAS.CST_NORD_EST;
                    return true;
                }
                if (angle < 112.5)
                {
                    direction = COMPAS.CST_NORD;
                    return true;
                }
                if (angle < 157.5)
                {
                    direction = COMPAS.CST_NORD_OUEST;
                    return true;
                }
                direction = COMPAS.CST_OUEST;
            }
            else
            {
                if (angle < 22.5)
                {
                    direction = COMPAS.CST_EST;
                    return true;
                }
                if (angle < 67.5)
                {
                    direction = COMPAS.CST_SUD_EST;
                    return true;
                }
                if (angle < 112.5)
                {
                    direction = COMPAS.CST_SUD;
                    return true;
                }
                if (angle < 157.5)
                {
                    direction = COMPAS.CST_SUD_OUEST;
                    return true;
                }
                direction = COMPAS.CST_OUEST;
            }
            return true;
        }

        public static bool CaseVersCase(int id_caseSource, int id_caseDestination, out string nomDistance)
        {
            int distance;
            double dist;

            nomDistance = string.Empty;
            Donnees.TAB_CASERow ligneCaseSource = Donnees.m_donnees.TAB_CASE.FindParID_CASE(id_caseSource);
            if (null == ligneCaseSource)
            {
                return false;
            }
            Donnees.TAB_CASERow ligneCaseDestination = Donnees.m_donnees.TAB_CASE.FindParID_CASE(id_caseDestination);
            if (null == ligneCaseDestination)
            {
                return false;
            }

            if (!CaseVersCompas(ligneCaseSource, ligneCaseDestination, out COMPAS direction))
            {
                return false;
            }
            dist = Constantes.Distance(ligneCaseSource.I_X, ligneCaseSource.I_Y, ligneCaseDestination.I_X, ligneCaseDestination.I_Y);
            distance = Convert.ToInt32(dist / Donnees.m_donnees.TAB_JEU[0].I_ECHELLE);
            nomDistance = string.Format("{0} km {1}", distance, DirectionOrdreVersCompasString(direction, true));
            return true;
        }

        /// <summary>
        /// renvoie la situation géographique la plus proche sous forme de texte par rapport à une case
        /// </summary>
        /// <param name="id_case">case origine</param>
        /// <param name="NomZoneGeographique">nom en texte de la zone</param>
        /// <returns>true si OK, false si KO</returns>
        public static bool CaseVersZoneGeographique(int id_case, out string nomZoneGeographique)
        {
            if (!CaseVersZoneGeographique(id_case, out decimal distance, out COMPAS direction, out int id_lieu))
            {
                nomZoneGeographique = string.Empty;
                return false;
            }
            return NomZoneGeographique(Math.Round(distance), direction, id_lieu, out nomZoneGeographique);
        }

        /// <summary>
        /// Recherche la zone géographique nommée la plus proche d'une case et en renvoie la distance, le nom et la direction
        /// </summary>
        /// <param name="id_case">case source</param>
        /// <param name="distance">distance entre le lieu et la case source</param>
        /// <param name="direction">direction vers la zone source</param>
        /// <param name="id_lieu">identifiant du lieu nommé</param>
        /// <returns></returns>
        public static bool CaseVersZoneGeographique(int id_case, out decimal distance, out COMPAS direction, out int id_lieu)
        {
            return CaseVersZoneGeographique(id_case, false, out distance, out direction, out id_lieu);
        }

        public static bool CaseVersZoneGeographique(int id_case, bool bSansPont, out decimal distance, out COMPAS direction, out int id_lieu)
        {
            id_lieu = -1;
            direction = COMPAS.CST_INDETERMINE;
            distance = int.MaxValue;
            double dist;
            int x=-1, y=-1;

            Donnees.TAB_CASERow ligneCaseSource = Donnees.m_donnees.TAB_CASE.FindParID_CASE(id_case);
            if (null == ligneCaseSource)
            {
                return false;
            }

            for (int l=0; l<Donnees.m_donnees.TAB_NOMS_CARTE.Count; l++)
            {
                Donnees.TAB_NOMS_CARTERow ligneNomCarte = Donnees.m_donnees.TAB_NOMS_CARTE[l];
                if (bSansPont && ligneNomCarte.S_NOM.ToUpper().Contains("PONT"))
                {
                    continue;
                }
                dist = Constantes.Distance(ligneCaseSource.I_X, ligneCaseSource.I_Y, ligneNomCarte.I_X, ligneNomCarte.I_Y) / Donnees.m_donnees.TAB_JEU[0].I_ECHELLE;
                if (dist < (double)distance)
                {
                    distance = (decimal)Convert.ToInt32(dist*10)/10;
                    id_lieu = ligneNomCarte.ID_NOM;
                    x = ligneNomCarte.I_X;
                    y = ligneNomCarte.I_Y;
                }
            }
            //if (distance > 0)
            //{
            //    Debug.Write("test");
            //}
            if (id_lieu >= 0 && distance >= 0)
            {
                return CaseVersCompas(x, y, ligneCaseSource.I_X, ligneCaseSource.I_Y, out direction);
            }
            return false;///aucun de lieu de trouvé, normalement impossible
        }

        /// <summary>
        /// Recherche la zone géographique nommée la plus proche d'une case et en renvoie le lieu le plus proche
        /// </summary>
        /// <param name="ligneCaseSource">case source</param>
        /// <param name="ligneNom">lieu nommé</param>
        /// <returns>true:OK, false:KO</returns>
        public static bool CaseVersZoneGeographique(Donnees.TAB_CASERow ligneCaseSource, out Donnees.TAB_NOMS_CARTERow ligneNom)
        {
            int distance = int.MaxValue;
            double dist;

            ligneNom = null;
            for (int l=0; l< Donnees.m_donnees.TAB_NOMS_CARTE.Count; l++)
            {
                Donnees.TAB_NOMS_CARTERow ligneNomCarte = Donnees.m_donnees.TAB_NOMS_CARTE[l];
                Donnees.TAB_CASERow ligneCase = Donnees.m_donnees.TAB_CASE.FindParID_CASE(ligneNomCarte.ID_CASE);
                if (null == ligneCase)
                {
                    return false;
                }
                dist = Constantes.Distance(ligneCaseSource.I_X, ligneCaseSource.I_Y, ligneCase.I_X, ligneCase.I_Y) / Donnees.m_donnees.TAB_JEU[0].I_ECHELLE;
                if (dist < distance)
                {
                    distance = Convert.ToInt32(dist);
                    ligneNom = ligneNomCarte;
                }
            }
            if (null == ligneNom)
            {
                return false;///aucun de lieu de trouvé, normalement impossible
            }
            return true;
        }

        /// <summary>
        /// Determine la position d'une case à partir de son indication géographique
        /// donnée par un ordre d'un joueur
        /// </summary>
        /// <param name="lignePion">pion qui chercher à déterminer la position</param>
        /// <param name="distance">distance par rapport au lieu en kilomètres</param>
        /// <param name="direction">direction au compas</param>
        /// <param name="id_lieu">lieu d'origine du point</param>
        /// <param name="id_case">case équivalente</param>
        /// <returns>true si on peut trouver le lieu, false sinon</returns>
        public static bool ZoneGeographiqueVersCase(Donnees.TAB_PIONRow lignePion, int distance, COMPAS direction, int id_lieu, out int id_case)
        {
            int x, y;
            //string requete;
            int echelle =Donnees.m_donnees.TAB_JEU[0].I_ECHELLE;
            double distanceMax;

            id_case = -1;
            if (distance < 0 || id_lieu < 0) { return false; }
            //calcul de la case 'brute'
            Donnees.TAB_NOMS_CARTERow ligneNomCarte = Donnees.m_donnees.TAB_NOMS_CARTE.FindByID_NOM(id_lieu);
            if (null == ligneNomCarte)
            {
                return false;
            }

            Donnees.TAB_CASERow ligneCaseSource = Donnees.m_donnees.TAB_CASE.FindParID_CASE(ligneNomCarte.ID_CASE);
            if (null == ligneCaseSource)
            {
                return false;
            }
            x = ligneCaseSource.I_X;
            y = ligneCaseSource.I_Y;
            switch (direction)
            {
                case COMPAS.CST_NORD:
                    y -= distance * Donnees.m_donnees.TAB_JEU[0].I_ECHELLE;
                    break;
                case COMPAS.CST_NORD_OUEST:
                    x -= (int)Math.Round(distance * echelle * degre45);
                    y -= (int)Math.Round(distance * echelle * degre45);
                    break;
                case COMPAS.CST_OUEST:
                    x -= distance * Donnees.m_donnees.TAB_JEU[0].I_ECHELLE;
                    break;
                case COMPAS.CST_SUD_OUEST:
                    x -= (int)Math.Round(distance * echelle * degre45);
                    y += (int)Math.Round(distance * echelle * degre45);
                    break;
                case COMPAS.CST_SUD:
                    y += distance * Donnees.m_donnees.TAB_JEU[0].I_ECHELLE;
                    break;
                case COMPAS.CST_SUD_EST:
                    x += (int)Math.Round(distance * echelle * degre45);
                    y += (int)Math.Round(distance * echelle * degre45);
                    break;
                case COMPAS.CST_EST:
                    x += distance * Donnees.m_donnees.TAB_JEU[0].I_ECHELLE;
                    break;
                case COMPAS.CST_NORD_EST:
                    x += (int)Math.Round(distance * echelle * degre45);
                    y -= (int)Math.Round(distance * echelle * degre45);
                    break;
                default:
                    break;
            }
            //si le point sort de la carte on le remet sur le bord !
            if (x < 0) { x = 0; }
            if (y < 0) { y = 0; }
            if (x > Donnees.m_donnees.TAB_JEU[0].I_LARGEUR_CARTE - 1) { x = Donnees.m_donnees.TAB_JEU[0].I_LARGEUR_CARTE - 1; }
            if (y > Donnees.m_donnees.TAB_JEU[0].I_HAUTEUR_CARTE - 1) { y = Donnees.m_donnees.TAB_JEU[0].I_HAUTEUR_CARTE - 1; }

            //on regarde s'il y a un lieu ou une route très proche (1 kilomètre), si oui, on recale la case dessus
            Donnees.TAB_CASERow[] ligneCaseResultat = Donnees.m_donnees.TAB_CASE.CasesCadre(
                Math.Max(0,x - echelle), Math.Max(0,y- echelle), 
                Math.Min(Donnees.m_donnees.TAB_JEU[0].I_LARGEUR_CARTE - 1, x + echelle), Math.Min(Donnees.m_donnees.TAB_JEU[0].I_HAUTEUR_CARTE - 1,y + echelle));
            if (0==ligneCaseResultat.Length)
            {
                return false;
            }

            /* Mauvaise idée, si tu veux te mettre à 1 km d'un lieu, tu te retrouves dessus ! 
            var result = from nomCarte in Donnees.m_donnees.TAB_NOMS_CARTE
                         join Case in ligneCaseResultat.AsEnumerable()
                         on nomCarte.ID_CASE equals Case.ID_CASE
                         select nomCarte.ID_CASE;

            if (result.Count() > 0)
            {
                //il existe un nom à proximité, on le renvoie
                id_case = result.ElementAt(0);
            }
            else
            {
             * */
                //on cherche la route la plus proche
                distanceMax = double.MaxValue;
                for (int i = 0; i < ligneCaseResultat.Length; i++)
                {
                    Donnees.TAB_MODELE_TERRAINRow ligneModeleTerrain=Donnees.m_donnees.TAB_MODELE_TERRAIN.FindByID_MODELE_TERRAIN(ligneCaseResultat[i].ID_MODELE_TERRAIN);
                    if (null != ligneModeleTerrain && ligneModeleTerrain.B_CIRCUIT_ROUTIER)
                    {
                        double d = Constantes.Distance(x, y, ligneCaseResultat[i].I_X, ligneCaseResultat[i].I_Y);
                        if (d < distanceMax)
                        {
                            distanceMax=d;
                            id_case = ligneCaseResultat[i].ID_CASE;
                        }
                    }
                }
            //}

            //si l'on a trouvé ni lieu, ni route, on renvoit la case exacte (en plein champ)
            if (id_case == -1)
            {
                Donnees.TAB_CASERow ligneCaseFinale = Donnees.m_donnees.TAB_CASE.FindParXY(x, y);
                if (null == ligneCaseFinale)
                {
                    return false;
                }
                id_case = ligneCaseFinale.ID_CASE;

                //si la case n'est pas accessible, on recherche la case accessible la plus proche comme cible
                Donnees.TAB_MODELE_PIONRow ligneModelePion = lignePion.modelePion;
                if (Donnees.m_donnees.TAB_MOUVEMENT_COUT.CalculCout(0, ligneCaseFinale.ID_MODELE_TERRAIN, ligneModelePion.ID_MODELE_MOUVEMENT) <= 0)
                {
                    var result2 = from modeleTerrain in Donnees.m_donnees.TAB_MODELE_TERRAIN
                                  join lCase in ligneCaseResultat //Donnees.m_donnees.TAB_CASE
                                  on modeleTerrain.ID_MODELE_TERRAIN equals lCase.ID_MODELE_TERRAIN
                                  //join modelePion in Donnees.m_donnees.TAB_MODELE_PION
                                  //on lignePion.ID_MODELE_PION equals modelePion.ID_MODELE_PION
                                  join modeleMouvement in Donnees.m_donnees.TAB_MODELE_MOUVEMENT
                                  on ligneModelePion.ID_MODELE_MOUVEMENT equals modeleMouvement.ID_MODELE_MOUVEMENT
                                  join mouvementCout in Donnees.m_donnees.TAB_MOUVEMENT_COUT
                                  on new { modeleTerrain.ID_MODELE_TERRAIN, ligneModelePion.ID_MODELE_MOUVEMENT } equals new { mouvementCout.ID_MODELE_TERRAIN, mouvementCout.ID_MODELE_MOUVEMENT }
                                  where (mouvementCout.ID_METEO == 0) && (mouvementCout.I_COUT > 0)
                                  orderby Constantes.Distance(ligneCaseFinale.I_X, ligneCaseFinale.I_Y, lCase.I_X, lCase.I_Y)
                                  select lCase;

                    if (!result2.Any()) { return false; }

                    id_case = result2.ElementAt(0).ID_CASE;
                }
            }
            return true;
        }

        /// <summary>
        /// renvoie sous forme de chaine de caractères l'expression d'un lieu géographique
        /// </summary>
        /// <param name="distance">distance par rapport au point en kilomètres</param>
        /// <param name="direction">direction au compas</param>
        /// <param name="id_lieu">lieu d'origine du point</param>
        /// <param name="zoneGeographique">nom en caractères</param>
        /// <returns>true si ok, false si ko</returns>
        public static bool NomZoneGeographique(decimal distance, COMPAS direction, int id_lieu, out string zoneGeographique)
        {
            string strDirection;

            zoneGeographique = string.Empty;
            Donnees.TAB_NOMS_CARTERow ligneNomCarte = Donnees.m_donnees.TAB_NOMS_CARTE.FindByID_NOM(id_lieu);
            if (null == ligneNomCarte)
            {
                return false;
            }
            strDirection = DirectionOrdreVersCompasString(direction, true);
            if (distance > 0)
            {
                string formatChaine;
                if (Constantes.DebuteParUneVoyelle(ligneNomCarte.S_NOM))
                {
                    formatChaine =  "{0} km {1} d'{2}";
                }
                else
                {
                    formatChaine = EstNomLieuMasculin(ligneNomCarte.S_NOM) ? "{0} km {1} du {2}" : "{0} km {1} de {2}";
                }                
                zoneGeographique = string.Format(formatChaine, distance, strDirection, ligneNomCarte.S_NOM);
            }
            else
            {
                zoneGeographique = ligneNomCarte.S_NOM;
            }
            return true;
        }

        private static bool RechercheNomLieuProche(Donnees.TAB_CASERow ligneCaseSource, out int id_lieu, out string nomLieu)
        {
            double distance = double.MaxValue;
            double dist;
            id_lieu = -1;
            nomLieu = string.Empty;
            for (int l=0; l<Donnees.m_donnees.TAB_NOMS_CARTE.Count; l++)
            {
                Donnees.TAB_NOMS_CARTERow ligneNomCarte = Donnees.m_donnees.TAB_NOMS_CARTE[l];
                Donnees.TAB_CASERow ligneCase = Donnees.m_donnees.TAB_CASE.FindParID_CASE(ligneNomCarte.ID_CASE);
                if (null == ligneCaseSource)
                {
                    return false;
                }
                dist = Constantes.Distance(ligneCaseSource.I_X, ligneCaseSource.I_Y, ligneCase.I_X, ligneCase.I_Y);
                if (dist < distance)
                {
                    //distance = Convert.ToInt32(dist / Donnees.m_donnees.TAB_JEU[0].I_ECHELLE);
                    distance = dist;
                    id_lieu = ligneNomCarte.ID_NOM;
                    nomLieu = ligneNomCarte.S_NOM;
                }
            }
            return true;
        }

        public static bool EstNomLieuMasculin(string nomLieu)
        {
            bool bnomMasculin = false;
            for (int i = 0; i < lieuxMasculins.Length; i++)
            {
                string lieu = lieuxMasculins[i];
                if (nomLieu[..Math.Min(nomLieu.Length, lieu.Length)].Equals(lieu, StringComparison.CurrentCultureIgnoreCase))
                {
                    bnomMasculin = true;
                }
            }
            return bnomMasculin;
        }

        public static bool NomDeBataille(int id_case, out string nomDeBataille)
        {
            Donnees.TAB_CASERow ligneCaseSource = Donnees.m_donnees.TAB_CASE.FindParID_CASE(id_case);
            return NomDeBataille(ligneCaseSource, out nomDeBataille);
        }

        public static bool NomDeBataille(Donnees.TAB_CASERow ligneCaseSource, out string nomDeBataille)
        {
            nomDeBataille = "La bataille introuvable";

            if (null == ligneCaseSource)
            {
                return false;
            }

            bool bRecherhe = RechercheNomLieuProche(ligneCaseSource, out int id_lieu, out string nomLieu);
            if (!bRecherhe || id_lieu <0)
            {
                return false;
            }

            //on boucle sur le nom jusqu'à en trouver un qui n'a jamais été pris
            int u = 0;
            bool bUtilise = true;
            while (bUtilise)
            {
                bUtilise = false;
                //si le nom est femminin, on commence par "de", si le nom est masculin on commence par "du"
                //par défaut,on prend "de" comme pour les villes.
                nomDeBataille = (sequenceceFeminin[u].Length > 0) ? "la " + sequenceceFeminin[u] + " bataille" : "la bataille";

                if (EstNomLieuMasculin(nomLieu))
                {
                    nomDeBataille += string.Format(" du {0}", nomLieu);
                }
                else
                {
                    //si le nom commence par une voyelle, il faut mettre une apostrophe
                    int i = 0;
                    while (i < voyelles.Length && nomLieu[0] != voyelles[i] && nomLieu[0] != char.ToUpper(voyelles[i])) i++;
                    if (i < voyelles.Length)
                    {
                        nomDeBataille += string.Format(" d'{0}", nomLieu);
                    }
                    else
                    {
                        nomDeBataille += string.Format(" de {0}", nomLieu);
                    }
                }

                //on regarde si le nom trouvé n'existe pas déjà
                int j = 0;
                while (!bUtilise && j<Donnees.m_donnees.TAB_BATAILLE.Count)
                {
                    Donnees.TAB_BATAILLERow ligneBataille = Donnees.m_donnees.TAB_BATAILLE[j];
                    if (ligneBataille.S_NOM.Equals(nomDeBataille)) { bUtilise = true; u++; }
                    j++;
                }
                
            }
            return true;
        }

        public static bool NomDeDepot(Donnees.TAB_CASERow ligneCaseSource, out string nomDeDepot)
        {
            nomDeDepot = "La dépôt introuvable";

            if (null == ligneCaseSource)
            {
                return false;
            }

            bool bRecherhe = RechercheNomLieuProche(ligneCaseSource, out int id_lieu, out string nomLieu);
            if (!bRecherhe || id_lieu < 0)
            {
                return false;
            }

            //on boucle sur le nom jusqu'à en trouver un qui n'a jamais été pris
            int u = 0;
            bool bUtilise = true;
            Monitor.Enter(Donnees.m_donnees.TAB_NOMS_PIONS.Rows.SyncRoot);
            while (bUtilise)
            {
                bUtilise = false;
                //si le nom est femminin, on commence par "de", si le nom est masculin on commence par "du"
                //par défaut,on prend "de" comme pour les villes.
                nomDeDepot = (sequenceceMasculin[u].Length > 0) ? sequenceceMasculin[u] + " dépôt" : "Dépôt";

                if (EstNomLieuMasculin(nomLieu))
                {
                    nomDeDepot += string.Format(" du {0}", nomLieu);
                }
                else
                {
                    //si le nom commence par une voyelle, il faut mettre une apostrophe
                    int i = 0;
                    while (i < voyelles.Length && nomLieu[0] != voyelles[i] && nomLieu[0] != char.ToUpper(voyelles[i])) i++;
                    if (i < voyelles.Length)
                    {
                        nomDeDepot += string.Format(" d'{0}", nomLieu);
                    }
                    else
                    {
                        nomDeDepot += string.Format(" de {0}", nomLieu);
                    }
                }

                //on regarde si le nom trouvé n'existe pas déjà
                int j = 0;
                int nbNoms = Donnees.m_donnees.TAB_NOMS_PIONS.Count;
                while (!bUtilise && j < nbNoms)
                {
                    if (Donnees.m_donnees.TAB_NOMS_PIONS[j].S_NOM.Equals(nomDeDepot)) { bUtilise = true; u++; }
                    j++;
                }
            }
            Donnees.m_donnees.TAB_NOMS_PIONS.AddTAB_NOMS_PIONSRow(nomDeDepot);
            Monitor.Exit(Donnees.m_donnees.TAB_NOMS_PIONS.Rows.SyncRoot);
            return true;
        }

        /// <summary>
        /// renvoie la date et l'heure, format standard : 12 janvier 1805, 12h36
        /// </summary>
        /// <param name="bhasard">true si on choisit un format au hasard, false sinon </param>
        /// <returns></returns>
        public static string DateHeure(bool bhasard)
        {
            return DateHeure(Donnees.m_donnees.TAB_PARTIE[0].I_TOUR, Donnees.m_donnees.TAB_PARTIE[0].I_PHASE, bhasard);
        }

        public static string DateHeure(DateTime temps)
        {
            return temps.ToString("d MMMM yyyy, HH:mm");//12 janvier 1805, 12h36
        }

        public static DateTime DateHeure()
        {
            return DateHeure(
                Donnees.m_donnees.TAB_PARTIE[0].IsI_TOURNull() ? 0 : Donnees.m_donnees.TAB_PARTIE[0].I_TOUR,
                Donnees.m_donnees.TAB_PARTIE[0].IsI_PHASENull() ? 0 : Donnees.m_donnees.TAB_PARTIE[0].I_PHASE);
        }

        public static DateTime DateHeure(int tour, int phase)
        {
            //calcul de la date courante
            DateTime temps = Donnees.m_donnees.TAB_JEU[0].DT_INITIALE;
            temps = temps.AddHours(Donnees.m_donnees.TAB_JEU[0].I_HEURE_INITIALE);
            temps = temps.AddHours(tour);
            temps = temps.AddMinutes(phase * 60 / Donnees.m_donnees.TAB_JEU[0].I_NOMBRE_PHASES);
            return temps;
        }

        public static string DateHeure(int tour, int phase, bool bhasard)
        {
            string retour;

            //calcul de la date courante
            DateTime temps = DateHeure(tour, phase);

            //conversion sous forme de chaine
            if (!bhasard)
            {
                retour = DateHeure(temps);
            }
            else
            {
                Random de = new();
                retour = de.Next(3) switch
                {
                    0 => DateHeure(temps),
                    1 => temps.ToString("dddd d MMMM yyyy à HH:mm"),
                    _ => temps.ToString("dddd d MMMM") + " de l'an de grâce " + temps.ToString("yyyy, HH:mm"),
                };
            }
            return retour;
        }

        /// <summary>
        /// renvoie le tour et la phase courante à partir de l'heure de jeu
        /// </summary>
        /// <param name="dateJeu">Date courante</param>
        /// <param name="tour">tour courant</param>
        /// <param name="phase">phase courante</param>
        public static void TourPhase(DateTime dateJeu, out int tour, out int phase)
        {
            //le tour est le nombre d'heures entre la date de jeu et la date initiale
            TimeSpan tempsDifference = dateJeu.Subtract(Donnees.m_donnees.TAB_JEU[0].DT_INITIALE.AddHours(Donnees.m_donnees.TAB_JEU[0].I_HEURE_INITIALE));
            tour = tempsDifference.Days*24 + tempsDifference.Hours;
            phase = (tempsDifference.Minutes * 60 + tempsDifference.Seconds) * Donnees.m_donnees.TAB_JEU[0].I_NOMBRE_PHASES / 3600;
        }

        /// <summary>
        /// renvoie une date en format date pour SQL
        /// </summary>
        /// <param name="tour">tour</param>
        /// <param name="phase">phase</param>
        /// <returns>date au format chaine</returns>
        public static string DateHeureSQL(int tour, int phase)
        {
            DateTime temps = Donnees.m_donnees.TAB_JEU[0].DT_INITIALE;
            temps = temps.AddHours(Donnees.m_donnees.TAB_JEU[0].I_HEURE_INITIALE);
            temps = temps.AddHours(tour);
            temps = temps.AddMinutes(phase * 60 / Donnees.m_donnees.TAB_JEU[0].I_NOMBRE_PHASES);
            temps = temps.AddSeconds((phase * 3600 / Donnees.m_donnees.TAB_JEU[0].I_NOMBRE_PHASES) % 60);

            //'1805-06-15 02:04:18'
            return Constantes.DateHeureSQL(temps);
        }

        private static bool CriDeRalliement(int id_modelePion, out string criRalliement)
        {
            criRalliement = string.Empty;
            Donnees.TAB_MODELE_PIONRow ligneModelePion = Donnees.m_donnees.TAB_MODELE_PION.FindByID_MODELE_PION(id_modelePion);
            if (null == ligneModelePion)
            {
                return false;
            }
            criRalliement = ligneModelePion.S_CRI_RALLIEMENT;
            return true;
        }

        private static bool NomDuSuperieur(Donnees.TAB_PIONRow lignePion, out string nomDuSuperieur)
        {
            nomDuSuperieur = string.Empty;
            Donnees.TAB_PIONRow lignePionLeader;

            //s'il s'agit d'un joueur, son supérieur c'est lui-même, sauf  dans le cas d'un message personnel mais dans ce cas, on appelle
            //pas cette méthode liée à la génération automatique de phrases.
            if (null != Donnees.m_donnees.TAB_ROLE.TrouvePion(lignePion.ID_PION))
            {
                // Il s'agit d'un joueur
                lignePionLeader = lignePion;
            }
            else
            {
                lignePionLeader = Donnees.m_donnees.TAB_PION.FindByID_PION(lignePion.ID_PION_PROPRIETAIRE);
            }

            if (null == lignePionLeader)
            {
                return false;
            }
            nomDuSuperieur = lignePionLeader.S_NOM;
            return true;
        }

        public static bool EnvoyerMessageImmediat(Donnees.TAB_PIONRow lignePion, ClassMessager.MESSAGES typeMessage, string message="")
        {
            return EnvoyerMessage(lignePion, typeMessage, 0, 0, 0, 0, 0, null, null, null, null, -1, 0, 0, string.Empty, true, message, null);
        }

        public static bool EnvoyerMessageImmediat(Donnees.TAB_PIONRow lignePion, ClassMessager.MESSAGES typeMessage, Donnees.TAB_BATAILLERow ligneBataille)
        {
            return EnvoyerMessage(lignePion, typeMessage, 0, 0, 0, 0, 0, ligneBataille, null, null, null, -1, 0, 0, string.Empty, true, string.Empty, null);
        }

        public static bool EnvoyerMessage(Donnees.TAB_PIONRow lignePion, ClassMessager.MESSAGES typeMessage)
        {
            return EnvoyerMessage(lignePion, typeMessage, 0, 0, 0, 0, 0, null, null, null, null, -1, 0, 0, string.Empty, false, string.Empty, null);
        }

        public static bool EnvoyerMessage(Donnees.TAB_PIONRow lignePion, Donnees.TAB_CASERow ligneCaseDestruction, ClassMessager.MESSAGES typeMessage)
        {
            return EnvoyerMessage(lignePion, typeMessage, 0, 0, 0, 0, 0, null, null, ligneCaseDestruction, null, -1, 0, 0, string.Empty, false, string.Empty, null);
        }

        public static bool EnvoyerMessage(Donnees.TAB_PIONRow lignePion, Donnees.TAB_MODELE_TERRAINRow ligneModeleTerrainDestination, ClassMessager.MESSAGES typeMessage)
        {
            return EnvoyerMessage(lignePion, typeMessage, 0, 0, 0, 0, 0, null, null, null, ligneModeleTerrainDestination, -1, 0, 0, string.Empty, false, string.Empty, null);
        }

        public static bool EnvoyerMessageImmediat(Donnees.TAB_PIONRow lignePion, ClassMessager.MESSAGES typeMessage, int moralPerduOuGagne, Donnees.TAB_BATAILLERow ligneBataille)
        {
            return EnvoyerMessage(lignePion, typeMessage, 0, 0, 0, moralPerduOuGagne, 0, ligneBataille, null, null, null, -1, 0, 0, string.Empty, true, string.Empty, null);
        }

        public static bool EnvoyerMessage(Donnees.TAB_PIONRow lignePion, ClassMessager.MESSAGES typeMessage, int moralPerduOuGagne, Donnees.TAB_BATAILLERow ligneBataille)
        {
            return EnvoyerMessage(lignePion, typeMessage, 0, 0, 0, moralPerduOuGagne, 0, ligneBataille, null, null, null, -1, 0, 0, string.Empty, false, string.Empty, null);
        }

        public static bool EnvoyerMessage(Donnees.TAB_PIONRow lignePion, ClassMessager.MESSAGES typeMessage, int moralPerduOuGagne, int fatiguePerduOuGagne)
        {
            return EnvoyerMessage(lignePion, typeMessage, 0, 0, 0, moralPerduOuGagne, fatiguePerduOuGagne, null, null, null, null, -1, 0, 0, string.Empty, false, string.Empty, null);
        }

        public static bool EnvoyerMessage(Donnees.TAB_PIONRow lignePion, ClassMessager.MESSAGES typeMessage, Donnees.TAB_BATAILLERow ligneBataille)
        {
            return EnvoyerMessage(lignePion, typeMessage, 0, 0, 0, 0, 0, ligneBataille, null, null, null, -1, 0, 0, string.Empty, false, string.Empty, null);
        }

        public static bool EnvoyerMessage(Donnees.TAB_PIONRow lignePion, ClassMessager.MESSAGES typeMessage, int iPertesInfanterie, int iPertesCavalerie, int artilleriePerduOuGagne, int moralPerduOuGagne, int ravitaillementGagneOuPerdu, int materielGagneOuPerdu, Donnees.TAB_BATAILLERow ligneBataille, List<Donnees.TAB_PIONRow> lignePionsEnBataille)
        {
            return EnvoyerMessage(lignePion, typeMessage, iPertesInfanterie, iPertesCavalerie, artilleriePerduOuGagne, moralPerduOuGagne, 0, ligneBataille, null, null, null, -1, ravitaillementGagneOuPerdu, materielGagneOuPerdu, string.Empty, false, string.Empty, lignePionsEnBataille);
        }

        public static bool EnvoyerMessage(Donnees.TAB_PIONRow lignePion, Donnees.TAB_PIONRow lignePionCible, ClassMessager.MESSAGES typeMessage)
        {
            return EnvoyerMessage(lignePion, typeMessage, 0, 0, 0, 0, 0, null, lignePionCible, null, null, -1, 0, 0, string.Empty, false, string.Empty, null);
        }

        public static bool EnvoyerMessage(Donnees.TAB_PIONRow lignePion, decimal distanceRavitaillement, int ravitaillementGagneOuPerdu, int materielGagneOuPerdu, string depotRavitaillement, ClassMessager.MESSAGES typeMessage)
        {
            return EnvoyerMessage(lignePion, typeMessage, 0, 0, 0, 0, 0, null, null, null, null, distanceRavitaillement, ravitaillementGagneOuPerdu, materielGagneOuPerdu, depotRavitaillement, false, string.Empty, null);
        }

        public static bool EnvoyerMessage(Donnees.TAB_PIONRow lignePion, ClassMessager.MESSAGES typeMessage, int iPertesInfanterie, int iPertesCavalerie, int artilleriePerduOuGagne, int moralPerduOuGagne, int fatiguePerduOuGagne, Donnees.TAB_BATAILLERow ligneBataille, Donnees.TAB_PIONRow lignePionCible, Donnees.TAB_CASERow ligneCaseDestination, Donnees.TAB_MODELE_TERRAINRow ligneModeleTerrainDestination, decimal distanceRavitaillement, int ravitaillementGagneOuPerdu, int materielGagneOuPerdu, string depotRavitaillement, bool bImmediat, string message, List<Donnees.TAB_PIONRow> lignePionsEnBataille)
        {
            Donnees.TAB_PIONRow lignePionLeader;
            string phrase;

            if (string.Empty == message)
            {
                phrase = GenererPhrase(lignePion, typeMessage, iPertesInfanterie, iPertesCavalerie, artilleriePerduOuGagne, moralPerduOuGagne, fatiguePerduOuGagne, ligneBataille, lignePionCible, ligneCaseDestination, ligneModeleTerrainDestination, (int)distanceRavitaillement, ravitaillementGagneOuPerdu, materielGagneOuPerdu, depotRavitaillement, lignePionsEnBataille);
                if (string.Empty == phrase)
                {
                    LogFile.Notifier(string.Format("Phrase vide pour le message de type {0} envoyé par pion ID={1}", typeMessage, lignePion.ID_PION));
                    return false;
                }
            }
            else
            {
                phrase = message;
            }

            // Si le pion emetteur est le pion d'un joueur et qu'il ne s'agit pas d'un message personnel (donc écrit d'un joueur vers un autre joueur)
            // le message doit être transmis au joueur et non à son supérieur
            if (null != Donnees.m_donnees.TAB_ROLE.TrouvePion(lignePion.ID_PION) && typeMessage != MESSAGES.MESSAGE_PERSONNEL)
            {
                // Il s'agit d'un joueur
                lignePionLeader = lignePion;
            }
            else
            {
                //Donnees.TAB_MODELE_PIONRow ligneModelePion = Donnees.m_donnees.TAB_MODELE_PION.FindByID_MODELE_PION(lignePion.ID_MODELE_PION);
                lignePionLeader = Donnees.m_donnees.TAB_PION.FindByID_PION(lignePion.ID_PION_PROPRIETAIRE);
            }

            if (null == lignePionLeader)
            {
                LogFile.Notifier(string.Format("EnvoyerMessage : Pas d'envoi du message {0} car le pion ID={1} n'a pas de chef présent",phrase, lignePion.ID_PION));
                return true;//le chef n'est pas actuellement présent (sans doute un renfort), inutile donc d'envoyer le message
            }
            return EnvoyerMessage(lignePion, lignePionLeader, typeMessage, phrase, bImmediat);
        }

        public static bool EnvoyerMessage(Donnees.TAB_PIONRow lignePionEmetteur, Donnees.TAB_PIONRow lignePionDestinataire, ClassMessager.MESSAGES typeMessage, string phrase, bool bImmediat)
        {
            return EnvoyerMessage(lignePionEmetteur, lignePionDestinataire, typeMessage,  phrase,  bImmediat, null);
        }

        public static bool EnvoyerMessage(Donnees.TAB_PIONRow lignePionEmetteur, Donnees.TAB_PIONRow lignePionDestinataire, ClassMessager.MESSAGES typeMessage, string phrase, bool bImmediat, int? idCaseDestination)
        {
            bool bMessageDirect;
            Donnees.TAB_PIONRow lignePionMessager;
            Donnees.TAB_MODELE_PIONRow ligneModelePion = lignePionEmetteur.modelePion;

            if (!lignePionEmetteur.CasesDebutFin(out int iDCaseDebut, out int iDCaseFin)) {return false;}

            if (bImmediat)
            {
                bMessageDirect = true;
            }
            else
            {
                bMessageDirect = false;
                int IdBatailleEmetteur = lignePionEmetteur.ID_BATAILLE;
                int IdBatailleDestinataire = lignePionDestinataire.ID_BATAILLE;
                if (//null != Donnees.m_donnees.TAB_ROLE.TrouvePion(lignePionEmetteur.ID_PION) || //le pion emetteur est un joueur, mais un joueur n'écrit pas toujours en direct a un autre joueur !
                    //lignePionEmetteur.IsID_PION_PROPRIETAIRENull() ||
                    //lignePionEmetteur.ID_PION_PROPRIETAIRE == lignePionEmetteur.ID_PION || //29/10/2012, comprend pas déjà couvert par PionVoitPion je pense et, problème, quel que soit le destinataire d'un message envoyé par un général en chef arrive toujours immédiatement puisqu'il est son propre chef !
                    ((Constantes.NULLENTIER != IdBatailleEmetteur) && (Constantes.NULLENTIER != IdBatailleDestinataire) &&
                        IdBatailleDestinataire > 0 && IdBatailleEmetteur > 0 &&
                        IdBatailleDestinataire == IdBatailleEmetteur) || 
                    PionVoitPion(lignePionEmetteur, lignePionDestinataire) ||
                    (typeMessage == MESSAGES.MESSAGE_RENFORT))//  || (typeMessage == MESSAGES.MESSAGE_POSITION) -> pas une bonne idée, ce message peut être utilisé autrement qu'en mode direct
                    //|| (0 == Donnees.m_donnees.TAB_PARTIE[0].I_TOUR && 0 == Donnees.m_donnees.TAB_PARTIE[0].I_PHASE))//au début, les joueurs peuvent se parler librement, gerer directement par le web
                {
                    bMessageDirect = true;//c'est un joueur ou le chef est engagé dans le combat que l'unité
                }
            }

            int iTourSansRavitaillement = lignePionEmetteur.I_TOUR_SANS_RAVITAILLEMENT;
            int iTourBlessure = lignePionEmetteur.IsI_TOUR_BLESSURENull() ? 0 : lignePionEmetteur.I_TOUR_BLESSURE;
            if (bMessageDirect)
            {
                if (null == Donnees.m_donnees.TAB_ROLE.TrouvePion(lignePionDestinataire.ID_PION))
                {
                    //le pion destinataire n'est pas un joueur, il faut donc le renvoyer vers le leader suivant
                    string message = string.Format("{0},ID={1}, EnvoyerMessage change le destinataire du messsage nouveau ID={2}, ancien ID={3}",
                        lignePionEmetteur.S_NOM, lignePionEmetteur.ID_PION, lignePionDestinataire.ID_PION_PROPRIETAIRE, lignePionDestinataire.ID_PION);
                    LogFile.Notifier(message);

                    Donnees.TAB_PIONRow ligneNouveauPionDestinataire= Donnees.m_donnees.TAB_PION.FindByID_PION(lignePionDestinataire.ID_PION_PROPRIETAIRE);
                    if (null==ligneNouveauPionDestinataire)
                    {
                        message = string.Format("{0},ID={1}, EnvoyerMessage le destinataire {2} n'est pas un joueur et on ne peut en changer car il n'a pas de leader",
                            lignePionEmetteur.S_NOM, lignePionEmetteur.ID_PION, lignePionDestinataire.ID_PION);
                        LogFile.Notifier(message);
                    }
                    else
                    {
                        if (ligneNouveauPionDestinataire.B_DETRUIT)
                        {
                            message = string.Format("{0},ID={1}, EnvoyerMessage le destinataire {2} n'est pas un joueur, son nouveau destinataire est détruit, pas d'envoi de message.",
                                lignePionEmetteur.S_NOM, lignePionEmetteur.ID_PION, lignePionDestinataire.ID_PION);
                            LogFile.Notifier(message);
                            return true; //le destinataire, n'existe plus, inutile de lui écrire !
                        }
                        return EnvoyerMessage(lignePionEmetteur, ligneNouveauPionDestinataire, typeMessage, phrase, bImmediat);
                    }
                }

                //Le message est directement visible, le destinataire est un joueur 
                Monitor.Enter(Donnees.m_donnees.TAB_MESSAGE.Rows.SyncRoot);
                Donnees.TAB_MESSAGERow ligneMessageDirect = Donnees.m_donnees.TAB_MESSAGE.AjouterMessage(
                    lignePionEmetteur.ID_PION,
                    lignePionDestinataire.ID_PION,
                    (int)typeMessage,
                    Donnees.m_donnees.TAB_PARTIE[0].I_TOUR,//I_TOUR_ARRIVEE
                    Donnees.m_donnees.TAB_PARTIE[0].I_PHASE,
                    Donnees.m_donnees.TAB_PARTIE[0].I_TOUR,//I_TOUR_DEPART
                    Donnees.m_donnees.TAB_PARTIE[0].I_PHASE,
                    phrase,
                    lignePionEmetteur.I_INFANTERIE,
                    lignePionEmetteur.I_CAVALERIE,
                    lignePionEmetteur.I_ARTILLERIE,
                    lignePionEmetteur.I_FATIGUE,
                    lignePionEmetteur.Moral,
                    iTourSansRavitaillement,
                    -1,//ID_BATAILLE,
                    -1,//I_ZONE_BATAILLE,
                    lignePionEmetteur.I_TOUR_FUITE_RESTANT,
                    lignePionEmetteur.B_DETRUIT,
                    iDCaseDebut,
                    iDCaseFin,
                    lignePionEmetteur.I_NB_PHASES_MARCHE_JOUR,
                    lignePionEmetteur.I_NB_PHASES_MARCHE_NUIT,
                    lignePionEmetteur.I_NB_HEURES_COMBAT,
                    lignePionEmetteur.I_MATERIEL,
                    lignePionEmetteur.I_RAVITAILLEMENT,
                    lignePionEmetteur.I_SOLDATS_RAVITAILLES,
                    lignePionEmetteur.I_NB_HEURES_FORTIFICATION,
                    lignePionEmetteur.I_NIVEAU_FORTIFICATION,
                    lignePionEmetteur.I_DUREE_HORS_COMBAT,
                    iTourBlessure,
                    lignePionEmetteur.C_NIVEAU_DEPOT
                    );
                //important de séparer id_bataille et i_zone_bataille, car au début i_zone_bataille et null mais le pion est bien en bataille !
                int IdBataille = lignePionEmetteur.ID_BATAILLE;
                if (Constantes.NULLENTIER == IdBataille)
                {
                    ligneMessageDirect.SetID_BATAILLENull();
                }
                else
                {
                    ligneMessageDirect.ID_BATAILLE = IdBataille;
                }

                int iZoneBataille = lignePionEmetteur.I_ZONE_BATAILLE;
                if (Constantes.NULLENTIER == iZoneBataille)
                {
                    ligneMessageDirect.SetI_ZONE_BATAILLENull();
                }
                else
                {
                    ligneMessageDirect.I_ZONE_BATAILLE = iZoneBataille;
                }
                Monitor.Exit(Donnees.m_donnees.TAB_MESSAGE.Rows.SyncRoot);
                //s'il s'agit d'un message de transfert, les effets doivent être traitées immédiatement car l'ordre ne sera jamais "reçu"
                lignePionEmetteur.ReceptionMessageTransfert(ligneMessageDirect);
                LogFile.Notifier(string.Format("CreerMessager: envoi d'un message direct {0} au pion ID={1}", phrase, lignePionDestinataire.ID_PION));
            }
            else
            {
                //il faut créer un messager qui va apporter le message
                //recherche du modele de pion message de la nation
                //var resultAptitude = from aptitude in Donnees.m_donnees.TAB_APTITUDES
                //             where aptitude.S_NOM == "MESSAGER" 
                //             select aptitude.ID_APTITUDE;
                //if (0 == resultAptitude.Count())
                //{
                //    throw new Exception("Pas d'aptitude MESSAGER trouvée dans EnvoyerMessage");
                //}
                //foreach (int idaptitude in resultAptitude)
                //{
                //    Debug.Print("idaptitude=" + idaptitude);

                //    var resultAptitudePion = from AptitudesPion in Donnees.m_donnees.TAB_APTITUDES_PION
                //                 where AptitudesPion.ID_APTITUDE == idaptitude
                //                 select AptitudesPion.ID_MODELE_PION;

                //    if (0 == resultAptitudePion.Count())
                //    {
                //          throw new Exception("Pas de piont avec l'aptitude MESSAGER trouvé dans EnvoyerMessage");
                //    }
                //    foreach (int idmodele in resultAptitudePion)
                //    {
                //        Debug.Print("idmodele=" + idmodele);

                //        var result = from Modele in Donnees.m_donnees.TAB_MODELE_PION
                //                     where Modele.ID_MODELE_PION==idmodele
                //                     select Modele.ID_MODELE_PION;
                Verrou.Verrouiller(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
                //dans de rares cas, le destinataire peut bouger entre la création du messager et celle de l'ordre ce qui provoque des erreurs de mouvement
                lignePionMessager = CreerMessager(lignePionEmetteur);
                int id_Case_Destination = idCaseDestination ?? lignePionDestinataire.ID_CASE;
                Verrou.Deverrouiller(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
                if (null!=lignePionMessager)
                {
                    //il faut ajouter le message 
                    Monitor.Enter(Donnees.m_donnees.TAB_MESSAGE.Rows.SyncRoot);
                    Donnees.TAB_MESSAGERow ligneMessage = Donnees.m_donnees.TAB_MESSAGE.AjouterMessage(
                        lignePionEmetteur.ID_PION,
                        lignePionMessager.ID_PION,
                        (int)typeMessage,
                        -1,//I_TOUR_ARRIVEE
                        -1,
                        Donnees.m_donnees.TAB_PARTIE[0].I_TOUR,//I_TOUR_DEPART
                        Donnees.m_donnees.TAB_PARTIE[0].I_PHASE,
                        phrase,
                        lignePionEmetteur.I_INFANTERIE,
                        lignePionEmetteur.I_CAVALERIE,
                        lignePionEmetteur.I_ARTILLERIE,
                        lignePionEmetteur.I_FATIGUE,
                        lignePionEmetteur.Moral,
                        iTourSansRavitaillement,
                        -1,//ID_BATAILLE
                        -1,//I_ZONE_BATAILLE
                        lignePionEmetteur.I_TOUR_FUITE_RESTANT,
                        lignePionEmetteur.B_DETRUIT,
                        iDCaseDebut,
                        iDCaseFin,
                        lignePionEmetteur.I_NB_PHASES_MARCHE_JOUR,
                        lignePionEmetteur.I_NB_PHASES_MARCHE_NUIT,
                        lignePionEmetteur.I_NB_HEURES_COMBAT,
                        lignePionEmetteur.I_MATERIEL,
                        lignePionEmetteur.I_RAVITAILLEMENT,
                        lignePionEmetteur.I_SOLDATS_RAVITAILLES,
                        lignePionEmetteur.I_NB_HEURES_FORTIFICATION,
                        lignePionEmetteur.I_NIVEAU_FORTIFICATION,
                        lignePionEmetteur.I_DUREE_HORS_COMBAT,
                        iTourBlessure,
                        lignePionEmetteur.C_NIVEAU_DEPOT
                        );
                    ligneMessage.SetI_TOUR_ARRIVEENull();
                    ligneMessage.SetI_PHASE_ARRIVEENull();
                    //important de séparer id_bataille et i_zone_bataille, car au début i_zone_bataille et null mais le pion est bien en bataille !
                    int IdBataille = lignePionEmetteur.ID_BATAILLE;
                    if (Constantes.NULLENTIER == IdBataille)
                    {
                        ligneMessage.SetID_BATAILLENull();
                    }
                    else
                    {
                        ligneMessage.ID_BATAILLE = IdBataille;
                    }

                    int iZoneBataille = lignePionEmetteur.I_ZONE_BATAILLE;
                    if (Constantes.NULLENTIER == iZoneBataille)
                    {
                        ligneMessage.SetI_ZONE_BATAILLENull();
                    }
                    else
                    {
                        ligneMessage.I_ZONE_BATAILLE = iZoneBataille;
                    }
                    Monitor.Exit(Donnees.m_donnees.TAB_MESSAGE.Rows.SyncRoot);

                    //et maintenant, un ordre de mouvement
                    Monitor.Enter(Donnees.m_donnees.TAB_ORDRE.Rows.SyncRoot);
                    //Donnees.TAB_ORDRERow ligneOrdre = 
                        Donnees.m_donnees.TAB_ORDRE.AddTAB_ORDRERow(
                        Constantes.NULLENTIER,//id_ordre_transmis
                        Constantes.NULLENTIER,//id_ordre_suivant
                        Constantes.NULLENTIER,
                        Constantes.ORDRES.MESSAGE,
                        lignePionMessager.ID_PION,
                        lignePionMessager.ID_CASE,
                        0,//I_EFFECTIF_DEPART
                        id_Case_Destination,
                        Constantes.NULLENTIER,//id nom destination
                        0,//I_EFFECTIF_DESTINATION
                        Donnees.m_donnees.TAB_PARTIE[0].I_TOUR,//I_TOUR_DEBUT
                        Donnees.m_donnees.TAB_PARTIE[0].I_PHASE,//I_PHASE_DEBUT
                        Constantes.NULLENTIER,//I_TOUR_FIN
                        Constantes.NULLENTIER,//I_PHASE_FIN
                        ligneMessage.ID_MESSAGE,
                        lignePionDestinataire.ID_PION,
                        Constantes.NULLENTIER,//ID_CIBLE
                        Constantes.NULLENTIER,//ID_DESTINATAIRE_CIBLE
                        Constantes.NULLENTIER,//ID_BATAILLE
                        Constantes.NULLENTIER,//I_ZONE_BATAILLE
                        Constantes.NULLENTIER,//I_HEURE_DEBUT
                        Constantes.NULLENTIER,//I_DUREE
                        Constantes.NULLENTIER);//I_ENGAGEMENT
                    Monitor.Exit(Donnees.m_donnees.TAB_ORDRE.Rows.SyncRoot);
                    LogFile.Notifier(string.Format("CreerMessager: envoi d'un message indirect \"{0}\" ID={2} au pion ID={1} par le messager ID={3}", 
                        phrase, lignePionDestinataire.ID_PION, ligneMessage.ID_MESSAGE, lignePionMessager.ID_PION));
                }
                else
                {
                    //erreur grave
                    throw new Exception("Pas de modèle Messager trouvé dans EnvoyerMessage");
                    ///return false;
                }
            }

            // Dans le cas d'un message envoyé par une unités en bataille et que le destinataire n'est pas le chef de la bataille
            // le message est également envoyé en copie au chef de la bataille en mode direct
            // à condition que l'unité ne soit pas elle même un chef
            int IdBatailleEmetteur2 = lignePionEmetteur.ID_BATAILLE;
            if ((Constantes.NULLENTIER != IdBatailleEmetteur2) && !lignePionEmetteur.estJoueur)
            {
                Donnees.TAB_BATAILLERow ligneBataille = Donnees.m_donnees.TAB_BATAILLE.FindByID_BATAILLE(IdBatailleEmetteur2);
                int idLeader012 = ligneBataille.ID_LEADER_012;
                if ((Constantes.NULLENTIER!=idLeader012) &&
                     idLeader012 >= 0 &&
                    (ligneModelePion.ID_NATION == ligneBataille.ID_NATION_012) &&
                    (lignePionDestinataire.ID_PION != idLeader012))
                {
                    Donnees.TAB_PIONRow ligneNouveauPionDestinataire = Donnees.m_donnees.TAB_PION.FindByID_PION(idLeader012);
                    if (!EnvoyerMessage(lignePionEmetteur,ligneNouveauPionDestinataire, typeMessage, phrase, true))
                    {
                        return false;
                    }
                }
                else
                {
                    if (!ligneBataille.IsID_LEADER_345Null() &&
                        ligneBataille.ID_LEADER_345 >=0 &&
                        (ligneModelePion.ID_NATION == ligneBataille.ID_NATION_345) && 
                        (lignePionDestinataire.ID_PION!=ligneBataille.ID_LEADER_345))
                    {
                        Donnees.TAB_PIONRow ligneNouveauPionDestinataire= Donnees.m_donnees.TAB_PION.FindByID_PION(ligneBataille.ID_LEADER_345);
                        if (!ligneNouveauPionDestinataire.B_DETRUIT)//possible si le leader est blessé/tué au combat
                        {
                            if (!EnvoyerMessage(lignePionEmetteur, ligneNouveauPionDestinataire, typeMessage, phrase, true))
                            {
                                return false;
                            }
                        }
                    }
                }
            }

            return true;
        }

        public static Donnees.TAB_PIONRow CreerMessager(Donnees.TAB_PIONRow lignePionEmetteur)
        {
            Donnees.TAB_PIONRow lignePionMessager = null;

            //il faut créer un messager qui va apporter le message
            //recherche du modele de pion message de la nation
            //var resultAptitude = from aptitude in Donnees.m_donnees.TAB_APTITUDES
            //             where aptitude.S_NOM == "MESSAGER" 
            //             select aptitude.ID_APTITUDE;
            //if (0 == resultAptitude.Count())
            //{
            //    throw new Exception("Pas d'aptitude MESSAGER trouvée dans EnvoyerMessage");
            //}
            //foreach (int idaptitude in resultAptitude)
            //{
            //    Debug.Print("idaptitude=" + idaptitude);

            //    var resultAptitudePion = from AptitudesPion in Donnees.m_donnees.TAB_APTITUDES_PION
            //                 where AptitudesPion.ID_APTITUDE == idaptitude
            //                 select AptitudesPion.ID_MODELE_PION;

            //    if (0 == resultAptitudePion.Count())
            //    {
            //          throw new Exception("Pas de piont avec l'aptitude MESSAGER trouvé dans EnvoyerMessage");
            //    }
            //    foreach (int idmodele in resultAptitudePion)
            //    {
            //        Debug.Print("idmodele=" + idmodele);

            //        var result = from Modele in Donnees.m_donnees.TAB_MODELE_PION
            //                     where Modele.ID_MODELE_PION==idmodele
            //                     select Modele.ID_MODELE_PION;

            int idModeleMESSAGER = Donnees.m_donnees.TAB_MODELE_PION.RechercherModele(lignePionEmetteur.modelePion.ID_NATION, "MESSAGER");

            if (idModeleMESSAGER >= 0)
            {
                //on a trouvé le modèle, il faut créer le pion associé
                Verrou.Verrouiller(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
                lignePionMessager = Donnees.m_donnees.TAB_PION.AddTAB_PIONRow(
                    idModeleMESSAGER,
                    lignePionEmetteur.ID_PION, //lignePionEmetteur.ID_PION_PROPRIETAIRE, ne peut pas être bon, des fois, c'est "0" le propriétaire
                    -1,
                    -1,
                    "Messager du " + lignePionEmetteur.S_NOM,
                    0, 0, 0, 0, 0, 0, 0, 
                    100, //le moral doit être à 100, sinon il va ajouter +1km/h à la vitesse pour déroute
                    100, //le moral doit être à 100, sinon il va ajouter +1km/h à la vitesse pour déroute
                    0, 0, 0, 'Z', 0, 0, 0, 0,
                    lignePionEmetteur.ID_CASE,
                    0, //I_TOUR_SANS_RAVITAILLEMENT
                    0, -1,
                    0, //I_TOUR_RETRAITE_RESTANT
                    0, false, false, false, false, false,
                    false,//B_ENNEMI_OBSERVABLE
                    0,//I_MATERIEL,
                    0,//I_RAVITAILLEMENT,
                    false,//B_CAVALERIE_DE_LIGNE,
                    false,//B_CAVALERIE_LOURDE,
                    false,//B_GARDE,
                    false,//B_VIEILLE_GARDE,
                    0,//I_TOUR_CONVOI_CREE,
                    -1,//ID_DEPOT_SOURCE
                    0,//I_SOLDATS_RAVITAILLES,
                    0,//I_NB_HEURES_FORTIFICATION,
                    0,//I_NIVEAU_FORTIFICATION,
                    0,//ID_PION_REMPLACE,
                    0,//I_DUREE_HORS_COMBAT,
                    0,//I_TOUR_BLESSURE,
                    false,//B_BLESSES,
                    false,//B_PRISONNIERS,
                    false,//B_RENFORT
                    -1,//ID_LIEU_RATTACHEMENT,
                    'Z',//C_NIVEAU_DEPOT
                    -1,//ID_PION_ESCORTE, 
                    0,//I_INFANTERIE_ESCORTE, 
                    0,//I_CAVALERIE_ESCORTE
                    0,//I_MATERIEL_ESCORTE
                    0,//I_TOUR_DERNIER_RAVITAILLEMENT_DIRECT
                    0,//I_VICTOIRE
                    -1,//I_TRI
                    string.Empty,
                    -1
                    );
                lignePionMessager.SetID_ANCIEN_PION_PROPRIETAIRENull();
                lignePionMessager.SetID_NOUVEAU_PION_PROPRIETAIRENull();
                lignePionMessager.SetI_ZONE_BATAILLENull();
                lignePionMessager.SetID_BATAILLENull();
                lignePionMessager.SetID_LIEU_RATTACHEMENTNull();
                lignePionMessager.SetID_PION_ESCORTENull();
                lignePionMessager.SetID_DEPOT_SOURCENull();
                lignePionMessager.SetI_TRINull();
                lignePionMessager.SetI_TOUR_ENNEMI_OBSERVABLENull();
                Verrou.Deverrouiller(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
            }

            return lignePionMessager;
        }

        public static string GenererPhrase(Donnees.TAB_PIONRow lignePion, ClassMessager.MESSAGES typeMessage, int iPertesInfanterie, int iPertesCavalerie, int artilleriePerduOuGagne, int moralPerduOuGagne, int fatiguePerduOuGagne, Donnees.TAB_BATAILLERow ligneBataille, Donnees.TAB_PIONRow lignePionCible, Donnees.TAB_CASERow ligneCaseDestination, Donnees.TAB_MODELE_TERRAINRow ligneModeleTerrainDestination, int distanceRavitaillement, int ravitaillementGagneOuPerdu, int materielGagneOuPerdu, string depotRavitaillement, List<Donnees.TAB_PIONRow> lignePionsEnBataille)
        {
            string phraseFormat = Donnees.m_donnees.TAB_PHRASE.DonneUnePhrase(typeMessage);
            string phrase;
            string nomZoneGeographique;
            string unitesAttaquantes, nationsAttaquantes;
            string ordreType="ordre non défini";
            string nomZoneGeographiqueOrdre="ordre zone géographique non définie";
            int ordreHeureDebut = 0;
            int ordreDuree = 0;
            string nomBataille = string.Empty;
            string distanceBataille = string.Empty;
            string messageInterception = string.Empty;
            string nomCible = string.Empty;
            Donnees.TAB_CASERow ligneCase; 
            Donnees.TAB_MODELE_TERRAINRow ligneModeleTerrain;
            string nomDuChefRemplace = "chef non défini";

            if (string.Empty == phraseFormat)
            {
                LogFile.Notifier(string.Format("GenererPhrase Il n'y a aucune phrase type définie pour le typeMessage = {0}",typeMessage));
                return string.Empty;
            }

            if (null != ligneBataille)
            {
                nomBataille = ligneBataille.S_NOM;

                Donnees.TAB_CASERow ligneCaseBataille = Donnees.m_donnees.TAB_CASE.FindParXY(
                    (ligneBataille.I_X_CASE_BAS_DROITE + ligneBataille.I_X_CASE_HAUT_GAUCHE) / 2,
                    (ligneBataille.I_Y_CASE_BAS_DROITE + ligneBataille.I_Y_CASE_HAUT_GAUCHE) / 2);

                if (!CaseVersCase(lignePion.ID_CASE, ligneCaseBataille.ID_CASE, out distanceBataille))
                {
                    LogFile.Notifier("GenererPhrase CaseVersCase renvoie une erreur");
                    return string.Empty;
                }
                //dans le cas où le pion est en bataille, on donne la position centrale de la bataille, sinon, le joueur a du mal parfois à entrer dans la zone
                if (lignePion.ID_BATAILLE == ligneBataille.ID_BATAILLE)
                {
                    CaseVersZoneGeographique(ligneCaseBataille.ID_CASE, out nomZoneGeographique);
                }
                else
                {
                    CaseVersZoneGeographique(lignePion.ID_CASE, out nomZoneGeographique);
                }
            }
            else
            {
                if ((MESSAGES.MESSAGE_BRUIT_DU_CANON == typeMessage 
                    || MESSAGES.MESSAGE_BRUIT_ENDOMMAGE_PONT == typeMessage 
                    || MESSAGES.MESSAGE_BRUIT_REPARER_PONT_PONTON == typeMessage)
                    && null != ligneCaseDestination)
                {
                    if (!CaseVersCase(lignePion.ID_CASE, ligneCaseDestination.ID_CASE, out distanceBataille))
                    {
                        LogFile.Notifier("GenererPhrase CaseVersCase renvoie une erreur en dehors d'une bataille");
                        return string.Empty;
                    }
                }
                CaseVersZoneGeographique(lignePion.ID_CASE, out nomZoneGeographique);
            }
            
            string nomModelePionSource = string.Empty;
            if (null != lignePionCible)
            {
                if (lignePionCible.estMessager || lignePionCible.estPatrouille)
                {
                    nomModelePionSource = lignePionCible.proprietaire.modelePion.S_NOM;
                }
                else
                {
                    nomModelePionSource = lignePionCible.modelePion.S_NOM;
                }
                //nom de la cible
                nomCible = lignePionCible.S_NOM;
                if (lignePionCible.estMessager)
                {
                    //message capturé à retrouver
                    Donnees.TAB_ORDRERow ligneOrdreMessager = Donnees.m_donnees.TAB_ORDRE.Courant(lignePionCible.ID_PION);
                    Donnees.TAB_MESSAGERow ligneMessageInterception = ligneOrdreMessager.messageTransmis;
                    if (null != ligneMessageInterception)
                    {
                        messageInterception = ligneMessageInterception.S_TEXTE;
                    }
                    else
                    {
                        Donnees.TAB_ORDRERow ligneOrdreTransmis = ligneOrdreMessager.ordreTransmis;
                        messageInterception = MessageDecrivantUnOrdre(ligneOrdreTransmis, true);
                    }
                }
                else
                {
                    //cas du messager qui n'arrive pas à remettre son message
                    if (lignePion.estMessager)
                    {
                        //message capturé à retrouver
                        Donnees.TAB_ORDRERow ligneOrdreMessager = Donnees.m_donnees.TAB_ORDRE.Courant(lignePion.ID_PION);
                        Donnees.TAB_MESSAGERow ligneMessageInterception = ligneOrdreMessager.messageTransmis;
                        if (null != ligneMessageInterception)
                        {
                            messageInterception = ligneMessageInterception.S_TEXTE;
                        }
                        else
                        {
                            Donnees.TAB_ORDRERow ligneOrdreTransmis = ligneOrdreMessager.ordreTransmis;
                            messageInterception = MessageDecrivantUnOrdre(ligneOrdreTransmis, true);
                        }
                    }
                }
            }

            string effectifs = ChaineEffectifs(lignePion.infanterie, lignePion.cavalerie, lignePion.artillerie);
            string effectifsPerdus = ChaineEffectifs(iPertesInfanterie, iPertesCavalerie, artilleriePerduOuGagne);

            CriDeRalliement(lignePion.ID_MODELE_PION, out string criRalliement);
            NomDuSuperieur(lignePion, out string nomDuSuperieur);
            if (null== lignePionsEnBataille)
            {
                PionsAttaquants(lignePion, out unitesAttaquantes, out nationsAttaquantes);
            }
            else
            {
                PhrasesPionsNations(lignePionsEnBataille, out unitesAttaquantes, out nationsAttaquantes);
            }

            PionsEnvironnants(lignePion, typeMessage, ligneCaseDestination, false, out string unitesEnvironnantes, out _);
            Donnees.TAB_ORDRERow ligneOrdreCourant = Donnees.m_donnees.TAB_ORDRE.Courant(lignePion.ID_PION);//ID_PION_PROPRIETAIRE
            if (null != ligneOrdreCourant)
            {
                ordreType = OrdreType(ligneOrdreCourant);
                if (!ligneOrdreCourant.IsID_CASE_DESTINATIONNull())
                {
                    CaseVersZoneGeographique(ligneOrdreCourant.ID_CASE_DESTINATION, out nomZoneGeographiqueOrdre);
                }
                ordreHeureDebut = ligneOrdreCourant.IsI_HEURE_DEBUTNull() ? 0 : ligneOrdreCourant.I_HEURE_DEBUT;
                ordreDuree = ligneOrdreCourant.IsI_DUREENull() ? 0 : ligneOrdreCourant.I_DUREE;
            }
            else
            {
                //possible dans le cas d'un ordre d'arrêt, il n'y a plus d'ordre courant
                CaseVersZoneGeographique(lignePion.ID_CASE, out nomZoneGeographiqueOrdre);
                LogFile.Notifier(string.Format("GenererPhrase l'unité ID={0} n'a pas d'ordre courant", lignePion.ID_PION));
            }

            int iTourSansRavitaillement = lignePion.I_TOUR_SANS_RAVITAILLEMENT;
            ligneCase = Donnees.m_donnees.TAB_CASE.FindParID_CASE(lignePion.ID_CASE);
            if (null == ligneModeleTerrainDestination)
            {
                ligneModeleTerrain = (null == ligneCaseDestination) ? Donnees.m_donnees.TAB_MODELE_TERRAIN.FindByID_MODELE_TERRAIN(ligneCase.ID_MODELE_TERRAIN) : Donnees.m_donnees.TAB_MODELE_TERRAIN.FindByID_MODELE_TERRAIN(ligneCaseDestination.ID_MODELE_TERRAIN);
            }
            else
            {
                ligneModeleTerrain = ligneModeleTerrainDestination;
            }

            //non du chef remplaçant, s'il existe
            Donnees.TAB_PIONRow lingePionRemplace= lignePion.pionRemplace;
            if (null != lingePionRemplace)
            {
                nomDuChefRemplace = lingePionRemplace.S_NOM;
            }
            

            phrase = string.Format(phraseFormat,
                DateHeure(true), //0
                lignePion.S_NOM, //1
                nomZoneGeographique,
                nomBataille, //3, sous la forme "la bataille de...
                iPertesInfanterie,
                lignePion.infanterie,//.I_INFANTERIE,//5
                iPertesCavalerie,
                lignePion.cavalerie,//.I_CAVALERIE,//7
                artilleriePerduOuGagne,
                lignePion.artillerie,//.I_ARTILLERIE,
                moralPerduOuGagne,//10 
                lignePion.Moral,
                lignePion.I_MORAL_MAX,//12
                fatiguePerduOuGagne,
                lignePion.I_FATIGUE,//14
                iTourSansRavitaillement,
                criRalliement,//16
                nomDuSuperieur,
                unitesAttaquantes,//18
                nationsAttaquantes,
                Donnees.m_donnees.TAB_METEO[Donnees.m_donnees.TAB_PARTIE[0].ID_METEO].S_NOM,//20
                ordreType,
                unitesEnvironnantes,//22
                nomZoneGeographiqueOrdre,
                ordreHeureDebut,//24
                ordreDuree,
                distanceBataille,//26
                "<I>" + messageInterception + "</I>",//27
                effectifs,//28
                nomCible,
                distanceRavitaillement, //30
                depotRavitaillement,
                ligneModeleTerrain.S_NOM,
                effectifsPerdus,//33
                lignePion.I_RAVITAILLEMENT,
                lignePion.I_MATERIEL,//35
                ravitaillementGagneOuPerdu,
                materielGagneOuPerdu,//37
                nomDuChefRemplace,//38
                nomModelePionSource//39
            );

            return phrase;
        }

        private static string ChaineEffectifs(int infanterie, int cavalerie, int artillerie)
        {
            string effectifs = string.Empty;
            if (infanterie > 0) effectifs += infanterie + " fantassins";
            if (cavalerie > 0 && effectifs.Length > 0) effectifs += ", ";
            if (cavalerie > 0) effectifs += cavalerie + " cavaliers";
            if (artillerie > 0 && effectifs.Length > 0) effectifs += ", ";
            if (artillerie > 0) effectifs += artillerie + " canons";
            if (string.Empty == effectifs) effectifs = " personnels sans valeur miliaire";
            return effectifs;
        }

        private static string OrdreType(Donnees.TAB_ORDRERow ligneOrdre)
        {
            string retour;

            switch (ligneOrdre.I_ORDRE_TYPE)
            {
                case Constantes.ORDRES.MOUVEMENT:
                    retour = "mouvement";
                    break;
                case Constantes.ORDRES.MESSAGE:
                    retour = "envoi de message";
                    break;
                case Constantes.ORDRES.COMBAT:
                    retour = "combat";
                    break;
                case Constantes.ORDRES.RETRAITE:
                    retour = "retraite";
                    break;
                case Constantes.ORDRES.PATROUILLE:
                    retour = "patrouille";
                    break;
                case Constantes.ORDRES.CONSTRUIRE_PONTON:
                    retour = "construire un ponton";
                    break;
                case Constantes.ORDRES.ENDOMMAGER_PONT:
                    retour = "endommager un pont";
                    break;
                case Constantes.ORDRES.REPARER_PONT:
                    retour = "réparer un pont";
                    break;
                case Constantes.ORDRES.ARRET:
                    retour = "s'arrêter sur place";
                    break;
                case Constantes.ORDRES.TRANSFERER:
                    retour = "transférer une unité";
                    break;
                case Constantes.ORDRES.GENERERCONVOI:
                    retour = "créer un convoi";
                    break;
                case Constantes.ORDRES.RENFORCER:
                    retour = "renforcer une unité";
                    break;
                case Constantes.ORDRES.SEFORTIFIER:
                    retour = "construire des fortications";
                    break;
                case Constantes.ORDRES.ETABLIRDEPOT:
                    retour = "établir un dépôt";
                    break;
                case Constantes.ORDRES.RETRAIT:
                    retour = "retrait";
                    break;
                case Constantes.ORDRES.LIGNE_RAVITAILLEMENT:
                    retour = "alimenter une ligne de ravitaillement";
                    break;
                case Constantes.ORDRES.REDUIRE_DEPOT:
                    retour = "réduire un dépôt";
                    break;
                case Constantes.ORDRES.RAVITAILLEMENT_DIRECT:
                    retour = "se ravitailler en direct sur un dépôt";
                    break;
                case Constantes.ORDRES.SUIVRE_UNITE:
                    retour = "suivre une autre unité";
                    break;
                case Constantes.ORDRES.ATTAQUE_PROCHE:
                    retour = "attaquer à proximité";
                    break;
                default:
                    LogFile.Notifier("GenererPhrase Ordre inconnu reçu");
                    retour = "inconnu";
                    break;
            }
            return retour;
        }

        internal static string MessageDecrivantUnOrdre(Donnees.TAB_ORDRERow ligneOrdre, bool avecProprietaire)
        {
            string retour;// = "MessageDecrivantUnOrdre : ordre indescriptible";
            string zoneGeographique;
            Donnees.TAB_PIONRow lignePionDestinataire;
            if ((ligneOrdre.IsID_DESTINATAIRENull()))
            {
                //il doit s'agir d'un ordre direct ?
                lignePionDestinataire = Donnees.m_donnees.TAB_PION.FindByID_PION(ligneOrdre.ID_PION);
            }
            else
            {
                lignePionDestinataire = Donnees.m_donnees.TAB_PION.FindByID_PION(ligneOrdre.ID_DESTINATAIRE);
            }

            switch (ligneOrdre.I_ORDRE_TYPE)
            {
                case Constantes.ORDRES.MOUVEMENT:
                    CaseVersZoneGeographique(ligneOrdre.ID_CASE_DESTINATION,out zoneGeographique);
                    if (24 == ligneOrdre.I_DUREE)
                    {
                        //ordre de mouvement permanent, inutile d'indiquer depuis quand ou combien de temps
                        retour = avecProprietaire ?
                                    string.Format("{0} a pour ordre de faire mouvement vers {1}.",
                                    lignePionDestinataire.S_NOM,
                                    zoneGeographique)
                                :
                                    string.Format("mouvement vers {0}.",
                                    zoneGeographique);
                    }
                    else
                    {
                        retour = avecProprietaire ?
                                    string.Format("{0} a pour ordre de faire mouvement vers {1} à partir de {2}h00 durant {3} heures.",
                                    lignePionDestinataire.S_NOM,
                                    zoneGeographique,
                                    ligneOrdre.I_HEURE_DEBUT,
                                    ligneOrdre.I_DUREE)
                                :
                                    string.Format("mouvement vers {0} à partir de {1}h00 durant {2} heures.",
                                    zoneGeographique,
                                    ligneOrdre.I_HEURE_DEBUT,
                                    ligneOrdre.I_DUREE);
                    }
                    break;

                case Constantes.ORDRES.MESSAGE:
                    retour = "envoi de message : bug applicatif";//normalement traité précedemment
                    break;

                case Constantes.ORDRES.COMBAT:
                    retour = avecProprietaire ?
                                string.Format("{0} a pour ordre d'entrer immédiatement au combat.",
                                lignePionDestinataire.S_NOM)
                            :
                                "s'engager au combat.";
                    break;

                case Constantes.ORDRES.RETRAITE:
                    retour = avecProprietaire ?
                                string.Format("{0} doit immédiatement faire retraite et quitter la bataille en cours.",
                                lignePionDestinataire.S_NOM)
                             :
                                "faire retraite et quitter la bataille en cours.";
                    break;

                case Constantes.ORDRES.PATROUILLE:
                    CaseVersZoneGeographique(ligneOrdre.ID_CASE_DESTINATION,out zoneGeographique);
                    retour = string.Format("{0} a pour ordre d'aller patrouiller en {1}.",
                            lignePionDestinataire.S_NOM, zoneGeographique
                            );
                    break;

                case Constantes.ORDRES.CONSTRUIRE_PONTON:
                    CaseVersZoneGeographique(ligneOrdre.ID_CASE_DESTINATION, out _);
                    retour = avecProprietaire ?
                                string.Format("{0} a pour ordre de construire un ponton.",
                                lignePionDestinataire.S_NOM)
                             :
                                "construire un ponton.";
                    break;

                case Constantes.ORDRES.ENDOMMAGER_PONT:
                    retour = avecProprietaire ?
                                string.Format("{0} a pour ordre d'endommager un pont.",
                                lignePionDestinataire.S_NOM)
                             :
                                "endommager un pont.";
                    break;

                case Constantes.ORDRES.REPARER_PONT:
                    //ligneOrdre.ID_CASE_DEPART et non destination car il n'y a pas destination, sur un ordre de destruction de pont
                    // c'est la case la plus proche
                    CaseVersZoneGeographique(ligneOrdre.ID_CASE_DEPART, out _);
                    retour = avecProprietaire ?
                                string.Format("{0} a pour ordre de réparer un pont.",
                                lignePionDestinataire.S_NOM)
                            :
                                "réparer un pont.";
                    break;

                case Constantes.ORDRES.ARRET:
                    retour = avecProprietaire ?
                                string.Format("{0} a pour ordre de s'arrêter sur place.",
                                lignePionDestinataire.S_NOM)
                            :
                                string.Format("s'arrêter sur place.");
                    break;

                case Constantes.ORDRES.TRANSFERER:
                    Donnees.TAB_PIONRow lignePionCible = Donnees.m_donnees.TAB_PION.FindByID_PION(ligneOrdre.ID_CIBLE);
                    Donnees.TAB_PIONRow lignePionProprietaire = lignePionCible.proprietaire;
                    //Donnees.TAB_PIONRow lignePionDestinataireCible = Donnees.m_donnees.TAB_PION.FindByID_PION(ligneOrdre.ID_DESTINATAIRE_CIBLE); -> toujours null
                    retour = (avecProprietaire && null!= lignePionProprietaire) ?
                                string.Format("{0} doit transférer l'unité {1} à {2}.",
                                lignePionProprietaire.S_NOM, lignePionCible.S_NOM, lignePionDestinataire.S_NOM)
                            :
                                string.Format("transférer l'unité {0} à {1}.",
                                lignePionCible.S_NOM, lignePionDestinataire.S_NOM);
                    break;

                case Constantes.ORDRES.GENERERCONVOI:
                    retour = avecProprietaire ?
                                string.Format("{0} a pour ordre de constituer un convoi.",
                                lignePionDestinataire.S_NOM)
                            :
                                "constituer un convoi.";
                    break;

                case Constantes.ORDRES.RENFORCER:
                    retour = avecProprietaire ?
                                string.Format("{0} a pour ordre de renforcer une unité.",
                                lignePionDestinataire.S_NOM)
                            :
                                "renforcer une unité.";
                    break;
                case Constantes.ORDRES.SEFORTIFIER:
                    retour = avecProprietaire ?
                                string.Format("{0} a pour ordre de construire des fortifications.",
                                lignePionDestinataire.S_NOM)
                            :
                                "construire des fortifications.";
                    break;
                case Constantes.ORDRES.ETABLIRDEPOT:
                    retour = avecProprietaire ?
                                string.Format("{0} a pour ordre d'établir un dépôt.",
                                lignePionDestinataire.S_NOM)
                            :
                                "établir un dépôt.";
                    break;
                case Constantes.ORDRES.RETRAIT:
                    retour = avecProprietaire ?
                                string.Format("{0} doit immédiatement fait un retrait de sa zone de bataille en cours.",
                                lignePionDestinataire.S_NOM)
                             :
                                "faire un retrait de la zone de bataille en cours.";
                    break;
                case Constantes.ORDRES.LIGNE_RAVITAILLEMENT:
                    CaseVersZoneGeographique(ligneOrdre.ID_CASE_DESTINATION, out zoneGeographique);
                    retour = avecProprietaire ?
                                string.Format("{0} gère actuellement une ligne de ravitaillement permanente vers {1}.",
                                lignePionDestinataire.S_NOM, zoneGeographique)
                             :
                                string.Format("alimenter une ligne de ravitaillement vers {0}.", zoneGeographique);
                    break;
                case Constantes.ORDRES.REDUIRE_DEPOT:
                    retour = avecProprietaire ?
                                string.Format("{0} réduit le dépôt et génère un convoi.",
                                lignePionDestinataire.S_NOM)
                             :
                                "réduire le dépôt.";
                    break;
                case Constantes.ORDRES.RAVITAILLEMENT_DIRECT:
                    retour = avecProprietaire ?
                                string.Format("{0} se ravitaille en direct sur un dépôt.",
                                lignePionDestinataire.S_NOM)
                             :
                                "se ravitaille en direct sur un dépôt.";
                    break;
                case Constantes.ORDRES.SUIVRE_UNITE:
                    retour = avecProprietaire ?
                                string.Format("suit la {0}.",
                                lignePionDestinataire.S_NOM)
                             :
                                "suit une unité.";
                    break;
                case Constantes.ORDRES.ATTAQUE_PROCHE:
                    retour = "attaquer à proximité";
                    break;
                default:
                    LogFile.Notifier("MessageDecrivantUnOrdre Ordre inconnu reçu");
                    retour = "inconnu : bug applicatif";
                    break;
            }
            Donnees.TAB_ORDRERow ligneOrdreSuivant = ligneOrdre.ordreSuivant;
            if (null != ligneOrdreSuivant)
            { 
                retour += " puis "+MessageDecrivantUnOrdre(ligneOrdre.ordreSuivant, avecProprietaire);
            }
            return retour;
        }

        private static bool PionVoitPion(Donnees.TAB_PIONRow lignePion, Donnees.TAB_PIONRow lignePionCible)
        {
            int visionKM;
            double dist;

            visionKM = lignePion.vision * Donnees.m_donnees.TAB_JEU[0].I_ECHELLE;
            Donnees.TAB_CASERow ligneCase = Donnees.m_donnees.TAB_CASE.FindParID_CASE(lignePion.ID_CASE);
            Donnees.TAB_CASERow ligneCaseCible = Donnees.m_donnees.TAB_CASE.FindParID_CASE(lignePionCible.ID_CASE);
            dist = Constantes.Distance(ligneCaseCible.I_X, ligneCaseCible.I_Y, ligneCase.I_X, ligneCase.I_Y);
            if (dist <= visionKM)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Renvoie la liste de tous les pions environnants un pion
        /// </summary>
        /// <param name="lignePion">pion dont on cherche les voisins</param>
        /// <param name="typeMessage">type de message pour lequel on demande la liste</param>
        /// <param name="ligneCaseDestination">case à partir de laquelle on cherche les pions alentours</param>
        /// <param name="unitesEnvironnantes">chaine décrivant l'environnement</param>
        /// <param name="bEnDanger">true si l'unité voit des unités ennemies combattives sans voir d'unités amies combattives (utile pour savoir si un role a le droit de donner des ordres ou pas</param>
        /// <returns>true si ok, false sinon</returns>
        public static bool PionsEnvironnants(Donnees.TAB_PIONRow lignePion, MESSAGES typeMessage, Donnees.TAB_CASERow ligneCaseDestination, bool bUniquementCombattives, out string unitesEnvironnantes, out bool bEnDanger)
        {
            string NomZoneGeographique;
            string nomType, femminin;
            //string requete;

            //bool test = Constantes.DebuteParUneVoyelle("armand");
            //test = Constantes.DebuteParUneVoyelle("berger");
            //test = Constantes.DebuteParUneVoyelle("évident");
            //test = Constantes.DebuteParUneVoyelle("non");
            //test = Constantes.DebuteParUneVoyelle("OUI");
            //test = Constantes.DebuteParUneVoyelle("ô c'est difficile");
            unitesEnvironnantes = string.Empty;
            bEnDanger = false;
            bool bProtection = false;

            try
            {
                Donnees.TAB_MODELE_PIONRow ligneModelePion = lignePion.modelePion;
                if (null == ligneModelePion)
                {
                    return false;
                }

                Donnees.TAB_CASERow ligneCase = ligneCaseDestination ?? Donnees.m_donnees.TAB_CASE.FindParID_CASE(lignePion.ID_CASE);
                lignePion.CadreVision(ligneCase, out int xCaseHautGauche, out int yCaseHautGauche, out int xCaseBasDroite, out int yCaseBasDroite);

                Donnees.TAB_CASERow[] ligneCaseVues = Donnees.m_donnees.TAB_CASE.CasesCadre(xCaseHautGauche, yCaseHautGauche, xCaseBasDroite, yCaseBasDroite);

                Dictionary<int, Barycentre> unitesVisibles = new();
                //dans le cas d'une patrouille, sur 2-4 (2d6) elle ne voit pas l'ennemi même s'il existe
                if (typeMessage != MESSAGES.MESSAGE_PATROUILLE_RAPPORT || Constantes.JetDeDes(2) > 5)
                {
                    //on recherche toutes les unités visibles, et on se préparent à calculer le barycentre de leur position

                    for (int l = 0; l < ligneCaseVues.Length; l++)
                    {
                        Donnees.TAB_CASERow ligneCaseVue = ligneCaseVues[l];
                        if (!ligneCaseVue.IsID_PROPRIETAIRENull() && ligneCaseVue.ID_PROPRIETAIRE != lignePion.ID_PION)
                        {
                            if (unitesVisibles.TryGetValue(ligneCaseVue.ID_PROPRIETAIRE, out Barycentre bar))
                            {
                                bar.x += ligneCaseVue.I_X;
                                bar.y += ligneCaseVue.I_Y;
                                bar.nb++;
                                //unitesVisibles[ligneCaseVue.ID_PROPRIETAIRE] = bar;//peut-être pas utile, si bouge la reference
                            }
                            else
                            {
                                Barycentre barNouveau = new()
                                {
                                    x = ligneCaseVue.I_X,
                                    y = ligneCaseVue.I_Y,
                                    nb = 1
                                };
                                unitesVisibles.Add(ligneCaseVue.ID_PROPRIETAIRE, barNouveau);
                            }
                        }
                    }

                    //il est possible qu'une unité ne soit pas visible car n'ayant trouvée aucune case pour se placer, on ajoute donc toute unité dont l'emplacement est dans la zone de vue
                    for (int l = 0; l < Donnees.m_donnees.TAB_PION.Count; l++)
                    {
                        Donnees.TAB_PIONRow lignePionVue = Donnees.m_donnees.TAB_PION[l];
                        if (lignePionVue.B_DETRUIT) { continue; }
                        Donnees.TAB_CASERow ligneCasePion = Donnees.m_donnees.TAB_CASE.FindParID_CASE(lignePionVue.ID_CASE);
                        if (ligneCasePion.I_X >= xCaseHautGauche && ligneCasePion.I_Y >= yCaseHautGauche && ligneCasePion.I_X <= xCaseBasDroite && ligneCasePion.I_Y <= yCaseBasDroite)
                        {
                            if (lignePionVue.ID_PION != lignePion.ID_PION &&
                                !unitesVisibles.ContainsKey(lignePionVue.ID_PION))
                            {
                                Barycentre bar = new();
                                bar.x += ligneCasePion.I_X;
                                bar.y += ligneCasePion.I_Y;
                                bar.nb = 1;
                                unitesVisibles.Add(lignePionVue.ID_PION, bar);
                            }
                        }
                    }

                    if (typeMessage == MESSAGES.MESSAGE_POSITION_OFFICIERS)
                    {
                        //cas particulier où l'on donne les positions des chefs non présents des unités présentes
                        return PostionsOfficiersAmis(lignePion, unitesVisibles, out unitesEnvironnantes);
                    }

                    unitesEnvironnantes += "<UL>";
                    foreach (KeyValuePair<int, Barycentre> unite in unitesVisibles)
                    {
                        Donnees.TAB_PIONRow lignePionVoisin = Donnees.m_donnees.TAB_PION.FindByID_PION(unite.Key);

                        if (null == lignePionVoisin || lignePionVoisin.B_DETRUIT) { continue; }//note, null possible si j'ai détruit manuellement une unité
                        if (lignePion.ID_PION == lignePionVoisin.ID_PION) { continue; }
                        bool bEstcombattif = lignePionVoisin.estCombattifQG(false, true, true);
                        if (!bEstcombattif && bUniquementCombattives) { continue; }

                        if (bEstcombattif && (lignePionVoisin.nation == lignePion.nation))
                        {
                            bProtection = true;//il y a un ami pour le "protéger"
                        }
                        else
                        {
                            bEnDanger = true;
                        }

                        Donnees.TAB_CASERow ligneCaseVoisin = Donnees.m_donnees.TAB_CASE.FindParXY(unite.Value.x / unite.Value.nb, unite.Value.y / unite.Value.nb);
                        Donnees.TAB_NATIONRow ligneNationVoisin = lignePionVoisin.nation;
                        CaseVersZoneGeographique(ligneCaseVoisin.ID_CASE, out NomZoneGeographique);
                        unitesEnvironnantes += "<LI>";
                        //if (string.Empty != unitesEnvironnantes)
                        //{
                        //    unitesEnvironnantes += ", ";
                        //}
                        femminin = "";
                        if (lignePionVoisin.estQG)
                        {
                            nomType = "un état-major";

                            if ((typeMessage != MESSAGES.MESSAGE_PATROUILLE_RAPPORT
                                    && typeMessage != MESSAGES.MESSAGE_PATROUILLE_CONTACT_ENNEMI
                                    && typeMessage != MESSAGES.MESSAGE_ENNEMI_OBSERVE)
                                || Constantes.JetDeDes(1) > 5)
                            {
                                nomType += (Constantes.DebuteParUneVoyelle(lignePionVoisin.S_NOM)) ? " de l'" : " du ";
                                nomType += lignePionVoisin.S_NOM;
                            }
                        }
                        else
                        {
                            if (lignePionVoisin.estPatrouille)
                            {
                                nomType = "une patrouille";
                                femminin = "e";
                            }
                            else
                            {
                                if (lignePionVoisin.estMessager)
                                {
                                    nomType = "un aide de camp";
                                }
                                else
                                {
                                    if (lignePionVoisin.estDepot)
                                    {
                                        nomType = (ligneModelePion.ID_NATION == ligneNationVoisin.ID_NATION) ? lignePionVoisin.S_NOM : "un dépôt";
                                    }
                                    else
                                    {
                                        if (lignePionVoisin.estConvoi)
                                        {
                                            nomType = (ligneModelePion.ID_NATION == ligneNationVoisin.ID_NATION) ? lignePionVoisin.S_NOM : "un convoi";
                                        }
                                        else
                                        {
                                            if (lignePionVoisin.estPontonnier)
                                            {
                                                nomType = "des pontonniers";
                                            }
                                            else
                                            {
                                                nomType = (ligneModelePion.ID_NATION == ligneNationVoisin.ID_NATION || Constantes.JetDeDes(1) > 2) ? lignePionVoisin.S_NOM : "une formation";
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        if (0 == lignePionVoisin.I_INFANTERIE && 0 == lignePionVoisin.I_CAVALERIE && 0 == lignePionVoisin.I_ARTILLERIE)
                        {
                            if ((typeMessage != MESSAGES.MESSAGE_PATROUILLE_RAPPORT
                                    && typeMessage != MESSAGES.MESSAGE_PATROUILLE_CONTACT_ENNEMI
                                    && typeMessage != MESSAGES.MESSAGE_ENNEMI_OBSERVE)
                                || Constantes.JetDeDes(1) > 2)
                            {
                                if (lignePion.nation.ID_NATION == ligneNationVoisin.ID_NATION)
                                {
                                    //unitées de la même nation que l'observateur, on indique le proprietaire plutôt que la nationalité, on remonte jusqu'à ce que l'on trouve un joueur
                                    unitesEnvironnantes += nomType;
                                    if (lignePionVoisin.estConvoi) unitesEnvironnantes += "(" + lignePionVoisin.C_NIVEAU_DEPOT + ")";
                                    unitesEnvironnantes += lignePionVoisin.ChaineAppartenance();
                                    //unitesEnvironnantes += string.Format(" situé{1} à {0}  avec des effectifs de ", NomZoneGeographique, femminin);
                                    unitesEnvironnantes += string.Format(" situé{1} à {0}", NomZoneGeographique, femminin);
                                }
                                else
                                {
                                    string formatChaine = (Constantes.DebuteParUneVoyelle(ligneNationVoisin.S_NOM)) ? "{0} de l'{1} situé{3} à {2}" : "{0} de la {1} situé{3} à {2}";
                                    unitesEnvironnantes +=
                                        string.Format(formatChaine,
                                        nomType,
                                        ligneNationVoisin.S_NOM,
                                        NomZoneGeographique,
                                        femminin);
                                }
                            }
                            else
                            {
                                unitesEnvironnantes +=
                                    string.Format("Une unité située à {0}",
                                    NomZoneGeographique);
                            }
                        }
                        else
                        {
                            if (lignePion.nation.ID_NATION == ligneNationVoisin.ID_NATION)
                            {
                                //unitées de la même nation que l'observateur, on indique le proprietaire plutôt que la nationalité, on remonte jusqu'à ce que l'on trouve un joueur
                                unitesEnvironnantes += nomType;
                                unitesEnvironnantes += lignePionVoisin.ChaineAppartenance();
                                unitesEnvironnantes += string.Format(" situé{1} à {0}", NomZoneGeographique, femminin);
                            }
                            else
                            {
                                string formatChaine = (Constantes.DebuteParUneVoyelle(ligneNationVoisin.S_NOM)) ? "{0} arborant le drapeau de l'{1} " : "{0} arborant le drapeau de la {1} ";
                                unitesEnvironnantes +=
                                    string.Format(formatChaine,
                                    nomType,
                                    ligneNationVoisin.S_NOM);
                            }
                            int ecartInfanterie = 0, ecartCavalerie = 0, ecartArtillerie = 0;

                            if ((typeMessage != MESSAGES.MESSAGE_PATROUILLE_RAPPORT && typeMessage != MESSAGES.MESSAGE_PATROUILLE_CONTACT_ENNEMI
                                && typeMessage != MESSAGES.MESSAGE_ENNEMI_OBSERVE && typeMessage != MESSAGES.MESSAGE_ENNEMI_OBSERVE)
                                || Constantes.JetDeDes(1) > 5)
                            {
                                //rien, les chiffres donnés sont exacts
                            }
                            else
                            {
                                // Il y a une erreur comprise entre et % dans les estimations 
                                ecartInfanterie = lignePionVoisin.infanterie * (Constantes.JetDeDes(2) - 7) / 10 + 1000 * (Constantes.JetDeDes(1) - 3) / 10;
                                ecartCavalerie = lignePionVoisin.cavalerie * (Constantes.JetDeDes(2) - 7) / 10 + 1000 * (Constantes.JetDeDes(1) - 3) / 10;
                                ecartArtillerie = lignePionVoisin.artillerie * (Constantes.JetDeDes(2) - 7) / 10 + 10 * (Constantes.JetDeDes(1) - 3) / 10;
                            }

                            if ((lignePionVoisin.infanterie + ecartInfanterie > 0) || (lignePionVoisin.cavalerie + ecartCavalerie > 0) || (lignePionVoisin.artillerie + ecartArtillerie > 0))
                            {
                                unitesEnvironnantes += " avec des effectifs de ";

                                if (lignePionVoisin.infanterie + ecartInfanterie > 0)
                                {
                                    int estimationInfanterie = (lignePionVoisin.infanterie + ecartInfanterie < 100) ? lignePionVoisin.infanterie + ecartInfanterie : (int)Math.Round((decimal)(lignePionVoisin.infanterie + ecartInfanterie) / 100) * 100;
                                    unitesEnvironnantes += estimationInfanterie.ToString() + " fantassins";
                                }
                                if (lignePionVoisin.cavalerie + ecartCavalerie > 0)
                                {
                                    if (lignePionVoisin.infanterie + ecartInfanterie > 0)
                                    {
                                        if (lignePionVoisin.artillerie + ecartArtillerie > 0)
                                        {
                                            unitesEnvironnantes += ", ";
                                        }
                                        else
                                        {
                                            unitesEnvironnantes += " et ";
                                        }
                                    }
                                    int estimationCavalerie = (lignePionVoisin.cavalerie + ecartCavalerie < 100) ? lignePionVoisin.cavalerie + ecartCavalerie : (int)Math.Round((decimal)(lignePionVoisin.cavalerie + ecartCavalerie) / 100) * 100;
                                    unitesEnvironnantes += estimationCavalerie.ToString() + " cavaliers";
                                }
                                if (lignePionVoisin.artillerie + ecartArtillerie > 0)
                                {
                                    if ((lignePionVoisin.infanterie + ecartInfanterie) > 0 || (lignePionVoisin.cavalerie + ecartCavalerie) > 0)
                                    {
                                        unitesEnvironnantes += " et ";
                                    }
                                    unitesEnvironnantes += (lignePionVoisin.artillerie + ecartArtillerie).ToString() + " canons";
                                }
                            }
                            unitesEnvironnantes += string.Format(" située{0} à {1}", femminin, NomZoneGeographique);
                        }

                        //ajout de la direction du mouvement prise par l'unité
                        Donnees.TAB_ORDRERow ligneOrdre = Donnees.m_donnees.TAB_ORDRE.Courant(lignePionVoisin.ID_PION);
                        if (null == ligneOrdre)
                        {
                            unitesEnvironnantes += " à l'arrêt";
                        }
                        else
                        {
                            switch (ligneOrdre.I_ORDRE_TYPE)
                            {
                                case Constantes.ORDRES.MOUVEMENT:
                                case Constantes.ORDRES.MESSAGE:
                                case Constantes.ORDRES.PATROUILLE:
                                    if (lignePionVoisin.ID_CASE == ligneOrdre.ID_CASE_DESTINATION)
                                    {
                                        unitesEnvironnantes += " à l'arrêt";
                                    }
                                    else
                                    {
                                        CaseVersCompas(lignePionVoisin.ID_CASE, ligneOrdre.ID_CASE_DESTINATION, out COMPAS direction);
                                        unitesEnvironnantes += " en mouvement vers " + DirectionOrdreVersCompasString(direction, false);
                                    }
                                    break;
                                case Constantes.ORDRES.ATTAQUE_PROCHE:
                                    unitesEnvironnantes += "en attaque";
                                    break;
                                case Constantes.ORDRES.COMBAT:
                                    unitesEnvironnantes += " au combat";
                                    break;
                                case Constantes.ORDRES.RETRAITE:
                                case Constantes.ORDRES.RETRAIT:
                                    unitesEnvironnantes += " en retraite";
                                    break;
                                case Constantes.ORDRES.CONSTRUIRE_PONTON:
                                    unitesEnvironnantes += " construisant un ponton";
                                    break;
                                case Constantes.ORDRES.ENDOMMAGER_PONT:
                                    unitesEnvironnantes += " endommageant un pont";
                                    break;
                                case Constantes.ORDRES.REPARER_PONT:
                                    unitesEnvironnantes += " réparant un pont";
                                    break;
                                case Constantes.ORDRES.ARRET:
                                case Constantes.ORDRES.TRANSFERER:
                                case Constantes.ORDRES.GENERERCONVOI:
                                case Constantes.ORDRES.RENFORCER:
                                case Constantes.ORDRES.ETABLIRDEPOT:
                                case Constantes.ORDRES.LIGNE_RAVITAILLEMENT:
                                case Constantes.ORDRES.REDUIRE_DEPOT:
                                case Constantes.ORDRES.RAVITAILLEMENT_DIRECT:
                                    unitesEnvironnantes += " à l'arrêt";
                                    break;
                                case Constantes.ORDRES.SEFORTIFIER:
                                    unitesEnvironnantes += " construisant des fortifications";
                                    break;
                                case Constantes.ORDRES.SUIVRE_UNITE:
                                    unitesEnvironnantes += "suivant une unité";
                                    break;
                                default:
                                    LogFile.Notifier("PionsEnvironnants Ordre inconnu reçu");
                                    unitesEnvironnantes += "inconnu";
                                    break;
                            }
                        }
                        unitesEnvironnantes += ".</LI>";
                    }
                }

                if (bProtection)
                {
                    bEnDanger = false;
                }

                unitesEnvironnantes += "</UL>";
                if (0 == unitesVisibles.Count)
                {
                    CaseVersZoneGeographique(lignePion.ID_CASE, out NomZoneGeographique);
                    unitesEnvironnantes = string.Format("Aucune unité présente à {0} km autour de {1}", lignePion.vision, NomZoneGeographique);
                }

                //Ajout des informations d'état sur les ponts/gués environnants.
                if (!bUniquementCombattives)
                {
                    bool bPontOuGuet = false;
                    Dictionary<int, Donnees.TAB_CASERow> listePonts = new();
                    for (int l = 0; l < ligneCaseVues.Length; l++)
                    {
                        Donnees.TAB_CASERow ligneCaseVue = ligneCaseVues[l];
                        Donnees.TAB_MODELE_TERRAINRow ligneModeleTerrain = Donnees.m_donnees.TAB_MODELE_TERRAIN.FindByID_MODELE_TERRAIN(ligneCaseVue.ID_MODELE_TERRAIN);
                        if (ligneModeleTerrain.B_PONT || ligneModeleTerrain.B_PONTON)
                        {
                            //cette case fait-elle partie d'un pont déjà indiqué ?
                            if (!listePonts.ContainsKey(ligneCaseVue.ID_CASE))
                            {
                                //si la case est trouvé on recherche toutes les cases de même type contigues
                                List<Donnees.TAB_CASERow> listeCasesVoisines = new()
                                {
                                    ligneCaseVue
                                };
                                ligneCaseVue.ListeCasesVoisinesDeMemeType(ref listeCasesVoisines);
                                for (int ll = 0; ll < listeCasesVoisines.Count; ll++)
                                {
                                    Donnees.TAB_CASERow ligneCasePont = listeCasesVoisines[ll];
                                    listePonts.Add(ligneCasePont.ID_CASE, ligneCasePont);
                                }
                                if (!bPontOuGuet)
                                {
                                    unitesEnvironnantes += "<UL>";//avec un <br/> en plus cela fait un trop gros espace
                                    bPontOuGuet = true;
                                }

                                CaseVersZoneGeographique(ligneCaseVue.ID_CASE, out NomZoneGeographique);
                                //unitesEnvironnantes += "<LI>" + PremiereCaseMajuscule(ligneModeleTerrain.S_NOM) + " situé à " + NomZoneGeographique + ".</LI>";
                                unitesEnvironnantes += "<LI>Un " + ligneModeleTerrain.S_NOM + " situé à " + NomZoneGeographique + ".</LI>";
                            }
                        }
                    }
                    if (bPontOuGuet) { unitesEnvironnantes += "</UL>"; }
                }
                if (lignePion.estAuCombat) { bEnDanger = false; } //on est jamais en danger au combat c'est bien connu 
                if (lignePion.IsB_EN_DANGERNull())
                {
                    Verrou.Verrouiller(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
                    lignePion.B_EN_DANGER = bEnDanger;
                    Verrou.Deverrouiller(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
                }
            }
            catch (Exception ex)
            {
                string messageEX = string.Format("exception PionsEnvironnants {3} : {0} : {1} :{2}",
                       ex.Message, (null == ex.InnerException) ? "sans inner exception" : ex.InnerException.Message,
                       ex.StackTrace, ex.GetType().ToString());
                LogFile.Notifier(messageEX);
                throw;
            }
            return true;
        }

        /// <summary>
        /// Renvoi un ami protecteur = combattif le plus proche
        /// </summary>
        /// <param name="ligneCaseVue">case du pion cherchant un ami</param>
        /// <returns>ami le plus proche trouvé, null sinon</returns>
        public static Donnees.TAB_PIONRow AmiEnvironnant(Donnees.TAB_CASERow ligneCaseVue)
        {
            Donnees.TAB_PIONRow lignePionCase = null;
            if (!ligneCaseVue.IsID_PROPRIETAIRENull())
            {
                lignePionCase = Donnees.m_donnees.TAB_PION.FindByID_PION(ligneCaseVue.ID_PROPRIETAIRE);
            }
            if (!ligneCaseVue.IsID_NOUVEAU_PROPRIETAIRENull())
            {
                lignePionCase = Donnees.m_donnees.TAB_PION.FindByID_PION(ligneCaseVue.ID_NOUVEAU_PROPRIETAIRE);
            }
            return (null == lignePionCase) ? null : AmiEnvironnant(lignePionCase);
        }

        /// <summary>
        /// Renvoi un ami protecteur = combattif le plus proche
        /// </summary>
        /// <param name="lignePion">Pion cherchant un ami</param>
        /// <returns>ami le plus proche trouvé, null sinon</returns>
        public static Donnees.TAB_PIONRow AmiEnvironnant(Donnees.TAB_PIONRow lignePion)
        {
            Donnees.TAB_PIONRow lignePionAmiCombattif=null;
            double distanceMin = double.MaxValue;

            try
            {
                Donnees.TAB_MODELE_PIONRow ligneModelePion = lignePion.modelePion;
                if (null == ligneModelePion)
                {
                    return null;
                }

                Donnees.TAB_CASERow ligneCase = Donnees.m_donnees.TAB_CASE.FindParID_CASE(lignePion.ID_CASE);
                lignePion.CadreVision(ligneCase, out int xCaseHautGauche, out int yCaseHautGauche, out int xCaseBasDroite, out int yCaseBasDroite);

                Donnees.TAB_CASERow[] ligneCaseVues = Donnees.m_donnees.TAB_CASE.CasesCadre(xCaseHautGauche, yCaseHautGauche, xCaseBasDroite, yCaseBasDroite);

                Dictionary<int, Barycentre> unitesVisibles = new();
                for (int l = 0; l < ligneCaseVues.Length; l++)
                {
                    Donnees.TAB_CASERow ligneCaseVue = ligneCaseVues[l];
                    Donnees.TAB_PIONRow lignePionCase = null;
                    if (!ligneCaseVue.IsID_PROPRIETAIRENull() && ligneCaseVue.ID_PROPRIETAIRE != lignePion.ID_PION)
                    {
                        lignePionCase = Donnees.m_donnees.TAB_PION.FindByID_PION(ligneCaseVue.ID_PROPRIETAIRE);
                    }
                    if (!ligneCaseVue.IsID_NOUVEAU_PROPRIETAIRENull() && ligneCaseVue.ID_NOUVEAU_PROPRIETAIRE != lignePion.ID_PION)
                    {
                        lignePionCase = Donnees.m_donnees.TAB_PION.FindByID_PION(ligneCaseVue.ID_NOUVEAU_PROPRIETAIRE);
                    }

                    if (null != lignePionCase && !Donnees.TAB_PIONRow.estEnnemi(ligneCaseVue, ligneModelePion, true, false))
                    {
                        double distance = Constantes.Distance(ligneCase.I_X, ligneCase.I_Y, ligneCaseVue.I_X, ligneCaseVue.I_Y);
                        if (distance < distanceMin)
                        {
                            distanceMin = distance;
                            lignePionAmiCombattif = lignePionCase;
                        }
                    }
                }

                //il est possible qu'une unité ne soit pas visible car n'ayant trouvée aucune case pour se placer, on ajoute donc toute unité dont l'emplacement est dans la zone de vue
                for (int l = 0; l < Donnees.m_donnees.TAB_PION.Count; l++)
                {
                    Donnees.TAB_PIONRow lignePionVue = Donnees.m_donnees.TAB_PION[l];
                    if (lignePionVue.B_DETRUIT || !lignePionVue.estCombattif) { continue; }
                    Donnees.TAB_CASERow ligneCasePion = Donnees.m_donnees.TAB_CASE.FindParID_CASE(lignePionVue.ID_CASE);
                    if (ligneCasePion.I_X >= xCaseHautGauche && ligneCasePion.I_Y >= yCaseHautGauche && ligneCasePion.I_X <= xCaseBasDroite && ligneCasePion.I_Y <= yCaseBasDroite)
                    {
                        if (lignePionVue.ID_PION != lignePion.ID_PION && !lignePion.estEnnemi(lignePionVue))
                        {
                            double distance = Constantes.Distance(ligneCase.I_X, ligneCase.I_Y, ligneCasePion.I_X, ligneCasePion.I_Y);
                            if (distance < distanceMin)
                            {
                                distanceMin = distance;
                                lignePionAmiCombattif = lignePionVue;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string messageEX = string.Format("exception AmiEnvironnant {3} : {0} : {1} :{2}",
                       ex.Message, (null == ex.InnerException) ? "sans inner exception" : ex.InnerException.Message,
                       ex.StackTrace, ex.GetType().ToString());
                LogFile.Notifier(messageEX);
                throw;
            }
            return lignePionAmiCombattif;
        }

        private static bool PostionsOfficiersAmis(Donnees.TAB_PIONRow lignePion, Dictionary<int, Barycentre> unitesVisibles, out string positionOfficiers)
        {
            List<int> listeOfficiersIndiques = new();
            positionOfficiers = string.Empty;
            foreach (KeyValuePair<int, Barycentre> unite in unitesVisibles)
            {
                Donnees.TAB_PIONRow lignePionVoisin = Donnees.m_donnees.TAB_PION.FindByID_PION(unite.Key);
                if (null!= lignePionVoisin && !lignePionVoisin.estQG && !lignePionVoisin.estMessager && !lignePionVoisin.estPatrouille && !lignePionVoisin.estEnnemi(lignePion) && lignePionVoisin.proprietaire!=lignePion)
                {
                    Donnees.TAB_PIONRow lignePionVoisinLeader = lignePionVoisin.proprietaire;
                    //on vérifie que le leader n'est pas visible, sinon inutile de redonner la position
                    //inutile également de l'indiquer à nouveau si on l'a déjà fait !
                    if (lignePionVoisinLeader.estJoueur && !unitesVisibles.ContainsKey(lignePionVoisinLeader.ID_PION) && !listeOfficiersIndiques.Contains(lignePionVoisinLeader.ID_PION))
                    {
                        if (string.Empty != positionOfficiers) { positionOfficiers += ", "; }
                        CaseVersZoneGeographique(lignePionVoisinLeader.ID_CASE, out string NomZoneGeographique);
                        positionOfficiers += lignePionVoisinLeader.S_NOM + " à " + NomZoneGeographique;
                        listeOfficiersIndiques.Add(lignePionVoisinLeader.ID_PION);
                    }
                }
            }
            return true;
        }

        public static string PremiereCaseMajuscule(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }
            char[] a = s.ToCharArray();
            a[0] = char.ToUpper(a[0]);
            return new string(a);
        }

        [Obsolete("Ne plus utiliser")]
        public static bool PionsEnvironnantsParKm(Donnees.TAB_PIONRow lignePion, out string unitesEnvironnantes)
        {
            int visionKM;
            double dist;
            string NomZoneGeographique;
            string nomType, femminin;

            unitesEnvironnantes = string.Empty;
            Donnees.TAB_MODELE_PIONRow ligneModelePion = lignePion.modelePion;
            if (null == ligneModelePion)
            {
                return false;
            }
            visionKM = lignePion.vision * Donnees.m_donnees.TAB_JEU[0].I_ECHELLE;

            Donnees.TAB_CASERow ligneCase = Donnees.m_donnees.TAB_CASE.FindParID_CASE(lignePion.ID_CASE);
            for (int l=0; l<Donnees.m_donnees.TAB_PION.Count; l++)
            {
                Donnees.TAB_PIONRow lignePionVoisin = Donnees.m_donnees.TAB_PION[l];
                if (lignePionVoisin.B_DETRUIT) { continue; }
                if (lignePion.ID_PION == lignePionVoisin.ID_PION) { continue; }
                Donnees.TAB_CASERow ligneCaseVoisin = Donnees.m_donnees.TAB_CASE.FindParID_CASE(lignePionVoisin.ID_CASE);
                dist = Constantes.Distance(ligneCaseVoisin.I_X, ligneCaseVoisin.I_Y, ligneCase.I_X, ligneCase.I_Y);
                if (dist <= visionKM)
                {
                    Donnees.TAB_NATIONRow ligneNationVoisin = lignePionVoisin.nation;
                    CaseVersZoneGeographique(lignePionVoisin.ID_CASE, out NomZoneGeographique);
                    if (string.Empty != unitesEnvironnantes)
                    {
                        unitesEnvironnantes += ", ";
                    }
                    femminin = "";
                    if (lignePionVoisin.estQG)
                    {
                        nomType = "un état-major";
                    }
                    else
                    {
                        if (lignePionVoisin.estPatrouille)
                        {
                            nomType = "une patrouille";
                            femminin = "e";
                        }
                        else
                        {
                            if (lignePionVoisin.estMessager)
                            {
                                nomType = "un aide de camp";
                            }
                            else
                            {
                                if (ligneModelePion.ID_NATION == ligneNationVoisin.ID_NATION)
                                {
                                    nomType = lignePionVoisin.S_NOM;
                                }
                                else
                                {
                                    nomType = "une formation";
                                }
                            }
                        }
                    }
                    if (0 == lignePionVoisin.I_INFANTERIE && 0 == lignePionVoisin.I_CAVALERIE && 0 == lignePionVoisin.I_ARTILLERIE)
                    {
                        unitesEnvironnantes +=
                            string.Format("{0} de nationalité{3} {1} situé{3} à {2}",
                            nomType,
                            ligneNationVoisin.S_NOM,
                            NomZoneGeographique,
                            femminin);
                    }
                    else
                    {
                        unitesEnvironnantes +=
                            string.Format("{0} arborant le drapeau de {1} avec des effectifs de {2} fantassins, {3} cavaliers et {4} canons, situé à {5}",
                            nomType,
                            ligneNationVoisin.S_NOM,
                            lignePionVoisin.I_INFANTERIE,
                            lignePionVoisin.I_CAVALERIE,
                            lignePionVoisin.I_ARTILLERIE,
                            NomZoneGeographique);
                    }
                }
            }
            if (string.Empty == unitesEnvironnantes)
            {
                CaseVersZoneGeographique(lignePion.ID_CASE, out NomZoneGeographique);
                unitesEnvironnantes = string.Format(" aucune unité présente à {0} km autour de {1}", lignePion.vision, NomZoneGeographique);
            }
            return true;
        }

        private static bool PionsAttaquants(Donnees.TAB_PIONRow lignePion, out string unitesAttaquantes, out string nationsAttaquantes)
        {
            int[] des = new int[6];
            int[] effectifs = new int[6];
            int[] canons = new int[6];
            int[] modificateurs = new int[6];
            bool bZone012;

            unitesAttaquantes = "aucune unité attaquante";
            nationsAttaquantes = "aucune nation attaquante";

            if (lignePion.IsID_BATAILLENull() || lignePion.IsI_ZONE_BATAILLENull())
            {
                return true;
            }
            
            Donnees.TAB_BATAILLERow ligneBataille = Donnees.m_donnees.TAB_BATAILLE.FindByID_BATAILLE(lignePion.ID_BATAILLE);
            if (null == ligneBataille)
            {
                return false;
            }

            if (0 == lignePion.I_ZONE_BATAILLE || 1 == lignePion.I_ZONE_BATAILLE || 2 == lignePion.I_ZONE_BATAILLE)
            {
                bZone012 = false;//on cherche les pions du camp d'en face !
            }
            else
            {
                bZone012 = true;
            }
            if (!ligneBataille.RecherchePionsEnBatailleParZone(ligneBataille.ID_BATAILLE, bZone012, out _, ref des, ref modificateurs, ref effectifs, ref canons, out Donnees.TAB_PIONRow[] lignePionsEnBataille, true/*bEngagement*/, false/*bCombattif*/, true/*QG*/, true /*bArtillerie*/))
            {
                return false;
            }

            List<Donnees.TAB_PIONRow> listePionsEnBataille = new();
            foreach(Donnees.TAB_PIONRow lignePionListe in lignePionsEnBataille) { listePionsEnBataille.Add(lignePionListe); }
            PhrasesPionsNations(listePionsEnBataille, out unitesAttaquantes, out nationsAttaquantes);

            return true;
        }

        public static void PhrasesPionsNations(List<Donnees.TAB_PIONRow> lignePions, out string unites, out string nations)
        {
            Collection<int> liste_nations = new();
            unites = string.Empty;
            for (int i = 0; i < lignePions.Count; i++)
            {
                Donnees.TAB_PIONRow lignePion = lignePions[i];
                if (unites != string.Empty)
                {
                    if (i == lignePions.Count - 1)
                    {
                        unites += " et ";
                    }
                    else
                    {
                        unites += ", ";
                    }
                }
                unites += lignePion.S_NOM;
                Donnees.TAB_MODELE_PIONRow ligneModelePionEnnemi = lignePion.modelePion;
                if (!liste_nations.Contains(ligneModelePionEnnemi.ID_NATION))
                {
                    liste_nations.Add(ligneModelePionEnnemi.ID_NATION);
                }
            }

            nations = string.Empty;
            for (int i = 0; i < liste_nations.Count; i++)
            {
                Donnees.TAB_NATIONRow ligneNation = Donnees.m_donnees.TAB_NATION.FindByID_NATION(liste_nations[i]);
                if (nations != string.Empty)
                {
                    if (i == liste_nations.Count - 1)
                    {
                        nations += " et ";
                    }
                    else
                    {
                        nations += ", ";
                    }
                }
                nations += ligneNation.S_NOM;
            }
        }
    }
}
