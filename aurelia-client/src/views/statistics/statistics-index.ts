import { IRouter, HttpClient } from "aurelia";
import { IAppUserAnswerable } from "../../domain/IAnswerable";
import { AnswerableService } from "../../services/answerable-service";
import { AppState } from "../../state/app-state";

export class StatisticsIndex {
    private message: string = '';
    private service: AnswerableService
    private quizes: IAppUserAnswerable[] = []
    private polls: IAppUserAnswerable[] = []

    constructor(@IRouter private router: IRouter, private httpClient: HttpClient, private appState: AppState) {
        this.service = new AnswerableService(httpClient, appState)
    }

    async attached() {
        const response = await this.service.getUserAnswerables()

        if (response.data.length === 0) this.message = "No statistics to show!"

        this.quizes = response.data.filter(x => x.answerable.type == 0)
        this.polls = response.data.filter(x => x.answerable.type == 1)
    }
}