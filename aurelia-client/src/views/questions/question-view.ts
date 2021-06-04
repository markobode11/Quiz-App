import { IRouter, HttpClient, Params, IRouteViewModel } from "aurelia";
import { IAnswer, IAnswerCreate } from "../../domain/IAnswer";
import { IQuestion } from "../../domain/IQuestion";
import { QuestionService } from "../../services/question-service";
import { AppState } from "../../state/app-state";

export class QuestionView implements IRouteViewModel {
    private errorMessage: string = '';
    private message: string = '';
    private model: IQuestion = {
        id: 0,
        text: '',
        answers: [],
        type: 0
    }
    private service: QuestionService
    private answerCounter: number = 1

    constructor(@IRouter private router: IRouter, private httpClient: HttpClient, private appState: AppState) {
        this.service = new QuestionService(httpClient, appState)
    }

    async load(params: Params) {
        const response = await this.service.get(params[0]);

        if (response.statusCode === 200) {
            this.model = response.data
            this.answerCounter = this.model.answers.length
        } else {
            this.errorMessage = response.errorMessage
        }
    }

    async onSubmit() {
        this.errorMessage = '';
        if (!this.validate()) return

        const response = await this.service.update(this.model.id, { id: this.model.id, text: this.model.text, type: this.model.type, answers: [] });
        await this.addAnswers(this.model.answers.filter(x => x.id == 0))
        if (response.statusCode === 204) {
            this.message = "Saved!"
        } else {
            this.errorMessage = response.errorMessage
        }
    }

    async addAnswers(answerList: IAnswer[]) {
        const service = new QuestionService(this.httpClient, this.appState);
        for (let answer of answerList) {
            await service.addAnswer(this.model.id, { text: answer.text, isCorrect: answer.isCorrect } as IAnswerCreate)
        }
    }

    validate() {
        if (this.model.text == '') {
            this.errorMessage = "Text cannot be empty"
            return false
        }
        if (this.model.answers.filter(x => x.text == '').length !== 0) {
            this.errorMessage = "All answers must have text!"
            return
        }
        if (this.model.answers.filter(x => x.isCorrect == true).length !== 1) {
            this.errorMessage = "1 answer can be marked as correct!"
            return
        }
        return true
    }

    increaseAnswerCounter() {
        this.answerCounter++;
        this.model.answers.push({
            id: 0,
            text: '',
            questionId: 0,
            isCorrect: false
        })
    }

    async removeAnswer(index: string, answerId: number) {
        let response = {} as {
            statusCode: number;
            errorMessage: string;
        }

        if (answerId !== 0) {
            response = await this.service.removeAnswer(answerId)
        }
        if (this.model.answers[index].id == 0 || response.statusCode < 299) {
            document.getElementById(index).remove()
            this.answerCounter--;
            this.model.answers.splice(parseInt(index), 1)
        }
    }

    confirmDelete() {
        const deleteButton = document.getElementById(`delete`)
        const confirmDeleteButton = document.getElementById(`confirm-delete`)

        deleteButton!.style.display = 'none';
        confirmDeleteButton!.style.display = 'block';
    }

    async onDelete() {
        const response = await this.service.delete(this.model.id)
        if (response.statusCode === 204) {
            this.router.load("/answerable-router")
        } else {
            this.errorMessage = response.errorMessage!;
        }
    }
}