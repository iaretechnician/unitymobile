using System;
using System.Collections.Generic;
using UnityEngine;

namespace CoreTrivia
{
    [Serializable]
    public class LanguageFormat
    {
        public int id;
        public string name;
    }

    [Serializable]
    public class LanguageList
    {
        public List<LanguageFormat> Languages = new List<LanguageFormat>();
    }

    [Serializable]
    public class CategoryFormat
    {
        public int id;
        public string name;
        public bool enableTimer;
        public int timerAmount;
        public bool enableLives;
        public int livesAmount;
        public bool limitQuestions;
        public int questionLimit;
        public string modified;

        public Texture image;
    }

    [Serializable]
    public class CategoryList
    {
        public List<CategoryFormat> Categories = new List<CategoryFormat>();
    }

    [Serializable]
    public class QuestionFormat
    {
        public int id;
        public string question;
        public string explanation;
        public string type;
        public bool tofAnswer;
        public ImageFormat image;
        public List<AnswerFormat> answers = new List<AnswerFormat>();
    }

    [Serializable]
    public class QuestionList
    {
        public List<QuestionFormat> Questions = new List<QuestionFormat>();
    }

    [Serializable]
    public class ImageFormat
    {
        public string filename;
    }

    [Serializable]
    public class AnswerFormat
    {
        public string answer;
        public bool correct;
    }
}