import { IAnswer } from "../domain/IAnswer";

export interface INewQuestionModel {
    text: string,
    answers: IAnswer[]
}