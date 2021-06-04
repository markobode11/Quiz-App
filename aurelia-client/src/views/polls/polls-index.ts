import { HttpClient } from "aurelia";
import { EType } from "../../domain/enums/EType";
import { IAnswerable } from "../../domain/IAnswerable";
import { AnswerableService } from "../../services/answerable-service";
import { AppState } from "../../state/app-state";

export class PollsIndex {
    private isAdmin = false;
    private errorMessage: string = '';
    private message: string = '';
    private polls: IAnswerable[] = [];
    private service: AnswerableService

    constructor(private httpClient: HttpClient, private appState: AppState) {
    }

    async attached() {
        this.isAdmin = this.appState.isAdmin
        this.service = new AnswerableService(this.httpClient, this.appState);
        const data = await this.service.getAllByType(EType.Poll)
        if (data.statusCode === 200) {
            const pollsHelper = data.data
            for (let poll of pollsHelper) {
                const questionsInPoll = await this.service.getAllQuestionsForAnswerable(poll.id)
                if (questionsInPoll.data.length < 1) {
                    if (this.appState.isAdmin) {
                        this.polls.push(poll)
                        document.getElementById(poll.id.toString()).style.backgroundColor = "#FFc6c4"
                    } else {
                        const index = pollsHelper.indexOf(poll)
                        pollsHelper.splice(index, 1)
                    }
                } else {
                    this.polls.push(poll)
                }
            }
        } else {
            this.errorMessage = data.errorMessage!
        }
    }

    confirmDelete(id: number) {
        const deleteButton = document.getElementById(`${id}-delete`)
        const confirmDeleteButton = document.getElementById(`${id}-confirm-delete`)

        deleteButton!.style.display = 'none';
        confirmDeleteButton!.style.display = 'block';
    }

    async onDelete(id: number) {
        const response = await this.service.delete(id)
        if (response.statusCode === 204) {
            this.message = 'Deleted!'
            this.polls = [...this.polls.filter(x => x.id != id)]
        } else {
            this.errorMessage = response.errorMessage!;
        }
    }
}