import { IRouter, HttpClient, IRouteViewModel, Params } from "aurelia";
import { Button } from "bootstrap";
import { EType } from "../../domain/enums/EType";
import { IAnswerable } from "../../domain/IAnswerable";
import { IQuestion } from "../../domain/IQuestion";
import { AnswerableService } from "../../services/answerable-service";
import { QuestionService } from "../../services/question-service";
import { AppState } from "../../state/app-state";

export class Answer implements IRouteViewModel {
    private errorMessage: string = '';
    private model: IAnswerable
    private questions: IQuestion[]
    private service: AnswerableService
    private selectedAnswers: { questionId: number, answerId: number }[] = []
    private showResults: boolean = false
    private results: { correctAnswers: number, incorrectAnswers: number } = { correctAnswers: 0, incorrectAnswers: 0 }

    constructor(@IRouter private router: IRouter, private httpClient: HttpClient, private appState: AppState) {
        this.service = new AnswerableService(httpClient, appState)
    }

    async load(params: Params) {
        await this.loadAnswerable(params[0])
        await this.loadQuestions(params[0])
    }

    async loadAnswerable(id: string) {
        const response = await this.service.get(id);

        if (response.statusCode === 200) {
            this.model = response.data
        } else {
            this.errorMessage = response.errorMessage
        }
    }

    async loadQuestions(id: string) {
        const response = await this.service.getAllQuestionsForAnswerable(parseInt(id));

        if (response.statusCode === 200) {
            this.questions = response.data
            this.questions.forEach(x => this.selectedAnswers.push({ questionId: x.id, answerId: 0 }))
        } else {
            this.errorMessage = response.errorMessage
        }

        const questionService = new QuestionService(this.httpClient, this.appState)
        for (let question of this.questions) {
            question.answers = (await questionService.getAllAnswers(question.id)).data
        }
    }

    async submitAnswers(e: Event) {
        e.preventDefault()
        if (this.selectedAnswers.filter(x => x.answerId == 0).length !== 0) {
            this.errorMessage = "All questions must be answered!"
            return
        }
        this.errorMessage = ''

        if (this.model.type == EType.Poll) {
            this.router.load("/answerable-router")
            return
        } 

        this.calculateResults();

        (document.getElementById('submit') as HTMLButtonElement).disabled = true
        document.querySelectorAll('select').forEach(x => x.disabled = true)

        if (this.appState.token !== null) {
            this.saveResults();
        }
    }

    calculateResults() {
        for (let selected of this.selectedAnswers) {
            const question = this.questions.find(y => y.id == selected.questionId)
            const answer = question.answers.find(y => y.id == selected.answerId)
            answer.isCorrect ? this.results.correctAnswers++ : this.results.incorrectAnswers++
        }

        this.showResults = true;
    }

    async saveResults() {
        const response = await this.service.saveResultsForUser(this.appState.token,
            {
                answerableId: this.model.id,
                correctAnswers: this.results.correctAnswers,
                incorrectAnswers: this.results.incorrectAnswers
            });

        if (response.statusCode > 299) {
            this.errorMessage = response.errorMessage
        }
    }
}