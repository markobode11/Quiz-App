import { HttpClient } from "aurelia";
import { EType } from "../../domain/enums/EType";
import { IAnswerable } from "../../domain/IAnswerable";
import { AnswerableService } from "../../services/answerable-service";
import { AppState } from "../../state/app-state";

export class QuizesIndex {
    private isAdmin = false;
    private errorMessage: string = '';
    private message: string = '';
    private quizes: IAnswerable[] = [];
    private service: AnswerableService

    constructor(private httpClient: HttpClient, private appState: AppState) {
    }

    async attached() {
        this.isAdmin = this.appState.isAdmin
        this.service = new AnswerableService(this.httpClient, this.appState);
        const data = await this.service.getAllByType(EType.Quiz)
        if (data.statusCode === 200) {
            const quizesHelper = data.data

            for (let quiz of quizesHelper) {
                const questionsInQuiz = await this.service.getAllQuestionsForAnswerable(quiz.id)
                if (questionsInQuiz.data.length < 1) {
                    if (this.appState.isAdmin) {
                        this.quizes.push(quiz)
                        document.getElementById(quiz.id.toString()).style.backgroundColor = "#FFc6c4"
                    } else {
                        const index = quizesHelper.indexOf(quiz)
                        quizesHelper.splice(index, 1)
                    }
                } else {
                    this.quizes.push(quiz)
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
            this.quizes = [...this.quizes.filter(x => x.id != id)]
        } else {
            this.errorMessage = response.errorMessage!;
        }
    }
}