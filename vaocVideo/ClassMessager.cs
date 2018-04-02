using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.Threading;
using WaocLib;

namespace vaocVideo
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
        private static string[] lieuxMasculins= { "pont","carrefour", "chemin","fleuve" };
        private static char[] voyelles = { 'a','e','i','o','u','y','î','ï','é','è','ê','ù','à' };
        private const double degre45 = 0.70710678118654752440084436210485;
        private static string[] sequenceceFeminin = { "", "seconde", "troisième", "quatrième", "cinquième", "sixième", "septième", "huitième", "neuvième", "dizième", 
                                                        "onzième", "douzième", "treizième", "quatorzième", "quinzième", "seizième", "dixseptième", "dixhuitième", "dixneuvième", "vingtième" };
        private static string[] sequenceceMasculin = { "", "second", "troisième", "quatrième", "cinquième", "sixième", "septième", "huitième", "neuvième", "dizième", 
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
            //l'unité se rend par manque de ravitaillement
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
            SOINS_APRES_BATAILLE = 78,
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
            MESSAGE_CREATION_DEPOT_IMPOSSIBLE = 89
            //Les bâtiments présents à {2} ne permettent pas d'y créer un dépôt de grande taille. Le convoi doit se rendre ailleurs ou constituer un autre dépôt.
            //On ne peut pas créer un dépôt de grande taille ici, le chef du convoi est dans l'incapacité de renforcer le dépôt présent
            //Le dépôt ne peut pas décharger à {2}, il n'y a pas assez de place.
        }
        /*
               DateHeure(true), //0
                lignePion.S_NOM, //1
                nomZoneGeographique,
                nomBataille, //3, sous la forme "la bataille de...
                iPertesInfanterie,
                lignePion.I_INFANTERIE,//5
                iPertesCavalerie,
                lignePion.I_CAVALERIE,//7
                artilleriePerduOuGagne,
                lignePion.I_ARTILLERIE,
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
                messageInterception,//27
                effectifs,//28
                nomCible,
                distanceRavitaillement, //30
                depotRavitaillement,
                ModeleTerrain, //32
                effectifsPerdus //33
                lignePion.I_RAVITAILLEMENT,
                lignePion.I_MATERIEL,//35
                ravitaillementGagneOuPerdu,
                materielGagneOuPerdu//37
                nomDuChefRemplace//38
*/
        #endregion

        public static COMPAS DirectionOrdreVersCompas(int i_direction)
        {
            COMPAS compas;

            switch (i_direction)
            {
                case 0:
                    compas = ClassMessager.COMPAS.CST_NORD;
                    break;
                case 1:
                    compas = ClassMessager.COMPAS.CST_NORD_EST;
                    break;
                case 2:
                    compas = ClassMessager.COMPAS.CST_EST;
                    break;
                case 3:
                    compas = ClassMessager.COMPAS.CST_SUD_EST;
                    break;
                case 4:
                    compas = ClassMessager.COMPAS.CST_SUD;
                    break;
                case 5:
                    compas = ClassMessager.COMPAS.CST_SUD_OUEST;
                    break;
                case 6:
                    compas = ClassMessager.COMPAS.CST_OUEST;
                    break;
                case 7:
                    compas = ClassMessager.COMPAS.CST_NORD_OUEST;
                    break;
                default:
                    compas = ClassMessager.COMPAS.CST_INDETERMINE;
                    break;
            }
            return compas;
        }

        public static string DirectionOrdreVersCompasString(COMPAS compas, bool avecLiaison)
        {
            string strDirection;

            switch (compas)
            {
                case COMPAS.CST_NORD:

                    strDirection = (avecLiaison) ? "au Nord" : "le Nord";
                    break;
                case COMPAS.CST_NORD_OUEST:
                    strDirection = (avecLiaison) ? "au Nord-Ouest" : "le Nord-Ouest";
                    break;
                case COMPAS.CST_OUEST:
                    strDirection = (avecLiaison) ? "à l'Ouest" : "l'Ouest";
                    break;
                case COMPAS.CST_SUD_OUEST:
                    strDirection = (avecLiaison) ? "au Sud-Ouest": "le Sud-Ouest";
                    break;
                case COMPAS.CST_SUD:
                    strDirection = (avecLiaison) ? "au Sud" : "le Sud";
                    break;
                case COMPAS.CST_SUD_EST:
                    strDirection = (avecLiaison) ? "au Sud-Est" : "le Sud-Est";
                    break;
                case COMPAS.CST_EST:
                    strDirection = (avecLiaison) ? "à l'Est" : "l'Est";
                    break;
                case COMPAS.CST_NORD_EST:
                    strDirection = (avecLiaison) ? "au Nord-Est" : "le Nord-Est";
                    break;
                default:
                    strDirection = "Indéterminé";
                    break;
            }
            return strDirection;
        }

        public static bool CaseVersCompas(int ID_CaseSource, int ID_CaseDestination, out COMPAS direction)
        {
            Donnees.TAB_CASERow ligneCaseSource = Donnees.m_donnees.TAB_CASE.FindByID_CASE(ID_CaseSource);
            Donnees.TAB_CASERow ligneCaseDestination = Donnees.m_donnees.TAB_CASE.FindByID_CASE(ID_CaseDestination);
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
            direction = COMPAS.CST_INDETERMINE;
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
            COMPAS direction;

            nomDistance = string.Empty;
            Donnees.TAB_CASERow ligneCaseSource = Donnees.m_donnees.TAB_CASE.FindByID_CASE(id_caseSource);
            if (null == ligneCaseSource)
            {
                return false;
            }
            Donnees.TAB_CASERow ligneCaseDestination = Donnees.m_donnees.TAB_CASE.FindByID_CASE(id_caseDestination);
            if (null == ligneCaseDestination)
            {
                return false;
            }

            if (!CaseVersCompas(ligneCaseSource, ligneCaseDestination, out direction))
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
            decimal distance;
            COMPAS direction;
            int id_lieu;
            if (!CaseVersZoneGeographique(id_case, out distance, out direction, out id_lieu))
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

            Donnees.TAB_CASERow ligneCaseSource = Donnees.m_donnees.TAB_CASE.FindByID_CASE(id_case);
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
                Donnees.TAB_CASERow ligneCase = Donnees.m_donnees.TAB_CASE.FindByID_CASE(ligneNomCarte.ID_CASE);
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

            Donnees.TAB_CASERow ligneCaseSource = Donnees.m_donnees.TAB_CASE.FindByID_CASE(ligneNomCarte.ID_CASE);
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
            Donnees.TAB_CASERow[] ligneCaseResultat = Donnees.m_donnees.TAB_CASE.CasesCadre(x - echelle, y - echelle, x + echelle, y + echelle);
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
                Donnees.TAB_CASERow ligneCaseFinale = Donnees.m_donnees.TAB_CASE.FindByXY(x, y);
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

                    if (0 == result2.Count()) { return false; }

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
                Donnees.TAB_CASERow ligneCase = Donnees.m_donnees.TAB_CASE.FindByID_CASE(ligneNomCarte.ID_CASE);
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
                if (nomLieu.Substring(0, Math.Min(nomLieu.Length, lieu.Length)).Equals(lieu, StringComparison.CurrentCultureIgnoreCase))
                {
                    bnomMasculin = true;
                }
            }
            return bnomMasculin;
        }

        public static bool NomDeBataille(int id_case, out string nomDeBataille)
        {
            Donnees.TAB_CASERow ligneCaseSource = Donnees.m_donnees.TAB_CASE.FindByID_CASE(id_case);
            return NomDeBataille(ligneCaseSource, out nomDeBataille);
        }

        public static bool NomDeBataille(Donnees.TAB_CASERow ligneCaseSource, out string nomDeBataille)
        {
            int id_lieu = -1, i;
            string nomLieu=string.Empty;
            nomDeBataille = "La bataille introuvable";

            if (null == ligneCaseSource)
            {
                return false;
            }

            bool bRecherhe = RechercheNomLieuProche(ligneCaseSource, out id_lieu, out nomLieu);
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
                    i = 0;
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
                while (!bUtilise && j<Donnees.m_donnees.TAB_BATAILLE.Count())
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
            int id_lieu = -1, i;
            string nomLieu = string.Empty;
            nomDeDepot = "La dépôt introuvable";

            if (null == ligneCaseSource)
            {
                return false;
            }

            bool bRecherhe = RechercheNomLieuProche(ligneCaseSource, out id_lieu, out nomLieu);
            if (!bRecherhe || id_lieu < 0)
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
                nomDeDepot = (sequenceceMasculin[u].Length > 0) ? sequenceceMasculin[u] + " dépôt" : "Dépôt";

                if (EstNomLieuMasculin(nomLieu))
                {
                    nomDeDepot += string.Format(" du {0}", nomLieu);
                }
                else
                {
                    //si le nom commence par une voyelle, il faut mettre une apostrophe
                    i = 0;
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
                while (!bUtilise && j < Donnees.m_donnees.TAB_PION.Count())
                {
                    Donnees.TAB_PIONRow lignePionRecherche = Donnees.m_donnees.TAB_PION[j];
                    if (lignePionRecherche.S_NOM.Equals(nomDeDepot)) { bUtilise = true; u++; }
                    j++;
                }

            }
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
            string retour = string.Empty;

            //calcul de la date courante
            DateTime temps = DateHeure(tour, phase);

            //conversion sous forme de chaine
            if (!bhasard)
            {
                retour = DateHeure(temps);
            }
            else
            {
                Random de = new Random();
                switch (de.Next(3))
                {
                    case 0:
                        retour = DateHeure(temps);
                        break;
                    case 1:
                        retour = temps.ToString("dddd d MMMM yyyy à HH:mm");
                        break;
                    default:
                        retour = temps.ToString("dddd d MMMM") + " de l'an de grâce " + temps.ToString("yyyy, HH:mm");
                        break;
                }
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
            if (!Donnees.m_donnees.TAB_NATION[ligneModelePion.ID_NATION].IsS_CRI_RALLIEMENTNull())
            {
                criRalliement = Donnees.m_donnees.TAB_NATION[ligneModelePion.ID_NATION].S_CRI_RALLIEMENT;
            }
            return true;
        }

        private static bool NomDuSuperieur(Donnees.TAB_PIONRow lignePion, out string nomDuSuperieur)
        {
            nomDuSuperieur = string.Empty;
            Donnees.TAB_PIONRow lignePionLeader = null;

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

        internal static string MessageDecrivantUnOrdre(Donnees.TAB_ORDRERow ligneOrdre, bool avecProprietaire)
        {
            string retour = "MessageDecrivantUnOrdre : ordre indescriptible";
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
                                    string.Format("mouvement vers {1}.",
                                    lignePionDestinataire.S_NOM,
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
                                    string.Format("mouvement vers {1} à partir de {2}h00 durant {3} heures.",
                                    lignePionDestinataire.S_NOM,
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
                    CaseVersZoneGeographique(ligneOrdre.ID_CASE_DESTINATION, out zoneGeographique);
                    retour = avecProprietaire ?
                                string.Format("{0} a pour ordre de construire un ponton en {1}.",
                                lignePionDestinataire.S_NOM, zoneGeographique)
                             :
                                string.Format("construire un ponton en {1}.",
                                lignePionDestinataire.S_NOM, zoneGeographique);
                    break;

                case Constantes.ORDRES.ENDOMMAGER_PONT:
                    //ligneOrdre.ID_CASE_DEPART et non destination car il n'y a pas destination, sur un ordre de destruction de pont
                    // c'est la case la plus proche
                    CaseVersZoneGeographique(ligneOrdre.ID_CASE_DEPART, out zoneGeographique);
                    retour = avecProprietaire ?
                                string.Format("{0} a pour ordre d'endommager un pont en {1}.",
                                lignePionDestinataire.S_NOM, zoneGeographique)
                             :
                                string.Format("endommager un pont en {1}.",
                                lignePionDestinataire.S_NOM, zoneGeographique);
                    break;

                case Constantes.ORDRES.REPARER_PONT:
                    //ligneOrdre.ID_CASE_DEPART et non destination car il n'y a pas destination, sur un ordre de destruction de pont
                    // c'est la case la plus proche
                    CaseVersZoneGeographique(ligneOrdre.ID_CASE_DEPART, out zoneGeographique);
                    retour = avecProprietaire ?
                                string.Format("{0} a pour ordre de réparer un pont en {1}.",
                                lignePionDestinataire.S_NOM, zoneGeographique)
                            :
                                string.Format("réparer un pont en {1}.",
                                lignePionDestinataire.S_NOM, zoneGeographique);
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
                    Donnees.TAB_PIONRow lignePionDestinataireCible = Donnees.m_donnees.TAB_PION.FindByID_PION(ligneOrdre.ID_DESTINATAIRE_CIBLE);
                    retour = avecProprietaire ?
                                string.Format("{0} doit transférer l'unité {1} à {2}.",
                                lignePionDestinataire.S_NOM, lignePionCible.S_NOM, lignePionDestinataireCible.S_NOM)
                            :
                                string.Format("transférer l'unité {1} à {2}.",
                                lignePionDestinataire.S_NOM, lignePionCible.S_NOM, lignePionDestinataireCible.S_NOM);
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
                    retour = avecProprietaire ?
                                string.Format("{0} gère actuellement une ligne de ravitaillement permanente.",
                                lignePionDestinataire.S_NOM)
                             :
                                "alimenter une ligne de ravitaillement.";
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
            Donnees.TAB_CASERow ligneCase = Donnees.m_donnees.TAB_CASE.FindByID_CASE(lignePion.ID_CASE);
            Donnees.TAB_CASERow ligneCaseCible = Donnees.m_donnees.TAB_CASE.FindByID_CASE(lignePionCible.ID_CASE);
            dist = Constantes.Distance(ligneCaseCible.I_X, ligneCaseCible.I_Y, ligneCase.I_X, ligneCase.I_Y);
            if (dist <= visionKM)
            {
                return true;
            }
            return false;
        }
    }
}
