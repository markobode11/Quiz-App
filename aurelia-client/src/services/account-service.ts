import { IFetchResponse } from "../types/IFetchResponse";
import { IJwt } from "../types/IJwt";
import { IMessage } from "../types/IMessage";
import { HttpClient } from "aurelia";
import { ApiBaseUrl } from "../configuration";

export class AccountService {
    constructor(protected httpClient: HttpClient) {
    }


    async login(email: string, password: string): Promise<IFetchResponse<IJwt | IMessage>> {
        let url = ApiBaseUrl + "/account/login";
        let body = { email, password };
        return this.fetchInfo(body, url);
    }

    async register(email: string, password: string, firstname: string, lastname: string)
        : Promise<IFetchResponse<IJwt | IMessage>> {

        let url = ApiBaseUrl + "/account/register"
        let body = { email, password, firstname, lastname };
        return await this.fetchInfo(body, url);
    }


    async fetchInfo(body: Object, url: string): Promise<IFetchResponse<IJwt | IMessage>> {
        try {
            const response = await this.httpClient.post(url, JSON.stringify(body),
                {
                    cache: 'no-store',
                });

            if (response.ok) {
                const data = (await response.json()) as IJwt;
                return {
                    statusCode: response.status,
                    data: data,
                };
            }

            const data = (await response.json()).messages;

            return {
                statusCode: response.status,
                errorMessage: data.join(', '),
            };
        } catch (reason) {
            return {
                statusCode: 0,
                errorMessage: JSON.stringify(reason),
            };
        }
    }

}
