import { inject, HttpClient } from "aurelia"
import { ApiBaseUrl } from "../configuration"
import { EType } from "../domain/enums/EType"
import { IAnswerable, IAppUserAnswerable } from "../domain/IAnswerable"
import { IQuestion } from "../domain/IQuestion"
import { IStatistics } from "../domain/IStatistics"
import { AppState } from "../state/app-state"
import { IFetchResponse } from "../types/IFetchResponse"
import { IMessage } from "../types/IMessage"
import { BaseService } from "./base-service"

inject()
export class AnswerableService extends BaseService<IAnswerable> {

    constructor(protected httpClient: HttpClient, appState: AppState) {
        super(ApiBaseUrl + "/Answerables", httpClient, appState)
    }

    async getAllByType(type: EType): Promise<IFetchResponse<IAnswerable[]>> {
        return (await this.fetchData(this.apiEndpointUrl + "/All/" + type)) as IFetchResponse<IAnswerable[]>
    }

    async addExistingQuestion(questionId: number, answerableId: number): Promise<IFetchResponse<IMessage>> {
        const url = this.apiEndpointUrl + '/AddExistingQuestion/' + answerableId + "/" + questionId
        try {
            const response = await this.httpClient.post(url, JSON.stringify({}),
                {
                    cache: "no-store",
                    headers: this.authHeaders
                })
            return {
                statusCode: response.status,
            }
        } catch (error) {
            return {
                statusCode: 0,
                errorMessage: JSON.stringify(error)
            }
        }
    }

    async getAllQuestionsForAnswerable(answerableId: number): Promise<IFetchResponse<IQuestion[]>> {

        let url = this.apiEndpointUrl + "/AllQuestions/" + answerableId;

        return (await this.fetchData(url, true)) as unknown as IFetchResponse<IQuestion[]>;
    }

    async removeQuestion(answerableId: number, questionId: number) {
        let url = this.apiEndpointUrl + '/RemoveQuestion/' + answerableId + "/" + questionId
        try {
            const response = await fetch(url,
                {
                    method: "POST",
                    headers: this.authHeaders
                });

            return {
                statusCode: response.status,
                errorMessage: response.ok ? null : response.statusText
            };
        } catch (reason) {
            return {
                statusCode: 0,
                errorMessage: JSON.stringify(reason),
            };
        }
    }

    async saveResultsForUser(token: string, results: {answerableId: number, correctAnswers: number, incorrectAnswers: number}) {
        let url = ApiBaseUrl + "/AppUserAnswerables"
        try {
            const response = await fetch(url,
                {
                    method: "POST",
                    headers: 
                    {
                        'Authorization': 'Bearer ' + token,
                        "Content-type": "application/json"
                    },
                    body: JSON.stringify(results)
                });

            return {
                statusCode: response.status,
                errorMessage: response.ok ? null : response.statusText
            };
        } catch (reason) {
            return {
                statusCode: 0,
                errorMessage: JSON.stringify(reason),
            };
        }
    }

    async getUserAnswerables() {
        let url = ApiBaseUrl + "/AppUserAnswerables"

        return (await this.fetchData(url, true)) as unknown as IFetchResponse<IAppUserAnswerable[]>;
    }

    async getStatistics() {
        const url = this.apiEndpointUrl + "/Statistics"
        return (await this.fetchData(url, true)) as unknown as IFetchResponse<IStatistics[]>;
    }
}