﻿using AI.ExplainitALL.Metrics;
using AI.ExplainitALL.Metrics.SimAlgs;

NgramJaccardSim ngramJaccardSim = new NgramJaccardSim(3); 
CheckingForHallucinations checker = new CheckingForHallucinations(ngramJaccardSim);

string document = "Коты живут по одиночке. Чтобы научиться ходить – надо ходить, чтобы научиться подтягиваться – надо подтягиваться, чтобы научиться решать задачи по физике – надо решать задачи по физике. Так говорил преподаватель физики в моём университете, и эта аналогия применима и к программированию.\r\n\r\nМожно сколько угодно упираться в сухую теорию, но без применения своих знаний на практике научиться программировать невозможно. В этой статье я подобрал несколько проектов для начинающих python-разработчиков. Эти проекты помогут закрепить теорию, применить полученные знания на практике и набить руку в написании кода. Некоторые из них даже можно добавить в будущее портфолио. Я объясню, чем хорош каждый проект, какие навыки и темы он позволяет проработать, а также сориентирую какие библиотеки и технологии можно использовать для его реализации.\r\n\r\nЦель данного \"топа\" – это не создание самого оригинального портфолио и не перечисление уникальных проектов. Цель статьи разобраться в простых вещах, технологиях и темах, которые помогут развить практические навыки программирования. Поэтому не стоит ждать здесь сборку Оптимуса Прайма, программирование Звезды смерти и создание двигателя на китовом жире. Мы пройдёмся по простым, но в тоже время базовым вещам. Ведь как говорил один мой приятель: «Всё великое начинается с малого».";
string answer = "Чтобы научиться ходить надо ходить. Цель статьи разобраться в простых вещах которые помогут развить практические навыки программирования. Собаки ходят стаями и иногда едят котов";

var supp =  checker.GetSupportSeq(document, answer);
var h =  checker.GetHallucinationsProb(document, answer);

Console.WriteLine(h);