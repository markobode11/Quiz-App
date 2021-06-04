import { EType } from "./enums/EType";
import { IAnswer } from "./IAnswer";

export interface IQuestion {
    id: number,
    text: string,
    type: EType
    answers: IAnswer[]
}
