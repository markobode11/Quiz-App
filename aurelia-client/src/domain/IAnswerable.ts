import { EType } from "./enums/EType";

export interface IAnswerable {
    id: number,
    name: string,
    description: string,
    type: EType
}

export interface IAnswerableCreate {
    name: string,
    description: string,
    type: EType | null
}

export interface IAppUserAnswerable {
    id: number,
    appUserId: number
    answerableId: number,
    answerable: IAnswerable,
    correctAnswers: number,
    incorrectAnswers: number
}