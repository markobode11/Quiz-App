import { HttpClient, IRouter } from "aurelia";
import { IStatistics } from "../../domain/IStatistics";
import { AnswerableService } from "../../services/answerable-service";
import { AppState } from "../../state/app-state";

export class HomeIndex {
    private loggedIn: boolean
    private statistics: IStatistics[]

    constructor(@IRouter private router: IRouter, private appState: AppState, private httpClient: HttpClient) {
        this.loggedIn = appState.firstname !== '' ? true : false
    }

    async redirect() {
        await this.router.load('/identity-register')
    }

    async attached() {
        const service = new AnswerableService(this.httpClient, this.appState);
        this.statistics = (await service.getStatistics()).data;
    }
}