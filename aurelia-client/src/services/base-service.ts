import { HttpClient, json } from "aurelia";
import { IFetchResponse } from "../types/IFetchResponse";
import { AppState } from "./../state/app-state";

export class BaseService<TEntity> {

    constructor(protected apiEndpointUrl: string, protected httpClient: HttpClient, protected appState: AppState) {
        // apiEndpointUrl = https://xxx.xxx.xxx.xx/api/v1/ContactTypes

    }

    protected authHeaders = this.appState.token ?
        {
            'Authorization': 'Bearer ' + this.appState.token
        }
        :
        undefined;

    async getAll(): Promise<IFetchResponse<TEntity[]>> {
        let url = this.apiEndpointUrl;

        return (await this.fetchData(url)) as IFetchResponse<TEntity[]>;
    }

    async get(id: string): Promise<IFetchResponse<TEntity>> {
        let url = this.apiEndpointUrl;
        url = url + '/' + id;

        return (await this.fetchData(url)) as IFetchResponse<TEntity>;

    }

    async delete(id: number) {

        let url = this.apiEndpointUrl + '/' + id
        try {
            const response = await fetch(url,
                {
                    method: "DELETE",
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

    async create(body: Object): Promise<IFetchResponse<TEntity>> {

        try {
            const response = await this.httpClient.post(this.apiEndpointUrl, JSON.stringify(body),
                {
                    cache: "no-store",
                    headers: this.authHeaders
                });

            return {
                statusCode: response.status,
                errorMessage: response.ok ? '' : response.statusText,
                data: response.ok ? (await response.json()) as TEntity : undefined
            };
        } catch (reason) {
            return {
                statusCode: 0,
                errorMessage: JSON.stringify(reason),
            };
        }
    }

    async update(id: number, updatedEntity: TEntity): Promise<IFetchResponse<TEntity>> {
        const url = this.apiEndpointUrl + '/' + id
        try {
            const response = await this.httpClient.put(url, JSON.stringify(updatedEntity),
                {
                    cache: "no-store",
                    headers: this.authHeaders
                });
            
            return {
                statusCode: response.status,
                errorMessage: response.ok ? '' : response.statusText
            };
        } catch (reason) {
            return {
                statusCode: 0,
                errorMessage: JSON.stringify(reason),
            };
        }
    }

    async fetchData(url: string, returnArray = false): Promise<IFetchResponse<TEntity | TEntity[]>> {
        try {
            const response = await this.httpClient.fetch(url,
                {
                    cache: "no-store",
                    headers: this.authHeaders
                });
            if (response.ok) {
                const data = !returnArray ? (await response.json()) as TEntity : (await response.json()) as TEntity[];
                return {
                    statusCode: response.status,
                    data: data,
                };
            }

            return {
                statusCode: response.status,
                errorMessage: response.statusText,
            };
        } catch (reason) {
            return {
                statusCode: 0,
                errorMessage: JSON.stringify(reason),
            };
        }
    }
}
