import { inject, HttpClient } from "aurelia"
import { ApiBaseUrl } from "../configuration"
import { IAnswer, IAnswerCreate } from "../domain/IAnswer"
import { IQuestion } from "../domain/IQuestion"
import { AppState } from "../state/app-state"
import { IFetchResponse } from "../types/IFetchResponse"
import { BaseService } from "./base-service"

inject()
export class QuestionService extends BaseService<IQuestion> {

    constructor(protected httpClient: HttpClient,  appState: AppState) {
        super(ApiBaseUrl + "/Questions", httpClient, appState)
    }

    async addAnswer(questionId: number, answer: IAnswerCreate) {
        const url = this.apiEndpointUrl + "/AddAnswer/" + questionId
        try {
            const response = await this.httpClient.post(url, JSON.stringify(answer),
                {
                    cache: "no-store",
                    headers: this.authHeaders
                });

            return {
                statusCode: response.status,
                errorMessage: response.ok ? '' : response.statusText,
            };
        } catch (reason) {
            return {
                statusCode: 0,
                errorMessage: JSON.stringify(reason),
            };
        }
    }

    async removeAnswer(answerId: number) {
        const url = this.apiEndpointUrl + "/RemoveAnswer/" + answerId
        try {
            const response = await this.httpClient.post(url, JSON.stringify({}),
                {
                    cache: "no-store",
                    headers: this.authHeaders
                });

            return {
                statusCode: response.status,
                errorMessage: response.ok ? '' : response.statusText,
            };
        } catch (reason) {
            return {
                statusCode: 0,
                errorMessage: JSON.stringify(reason),
            };
        }
    }

    async getAllAnswers(questionId: number) {
        let url = this.apiEndpointUrl + "/QuestionAnswers/" + questionId;

        return (await this.fetchData(url, true)) as unknown as IFetchResponse<IAnswer[]>;
    }
}