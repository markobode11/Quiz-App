export interface IAnswer{
    id: number,
    text: string,
    isCorrect: boolean,
    questionId: number
}

export interface IAnswerCreate{
    text: string,
    isCorrect: boolean,
}

