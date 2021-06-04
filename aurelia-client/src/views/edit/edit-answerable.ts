import { HttpClient, IRouter, IRouteViewModel, Params } from "aurelia";
import { EType } from "../../domain/enums/EType";
import { IAnswer, IAnswerCreate } from "../../domain/IAnswer";
import { IAnswerable } from "../../domain/IAnswerable";
import { IQuestion } from "../../domain/IQuestion";
import { AnswerableService } from "../../services/answerable-service";
import { QuestionService } from "../../services/question-service";
import { AppState } from "../../state/app-state";
import { INewQuestionModel } from "../../types/INewQuestionModel";

export class EditAnswerable implements IRouteViewModel {
    private model: IAnswerable =
        {
            name: '',
            description: '',
            type: EType.Quiz,
            id: 0
        }

    private errorMessage: string = '';
    private secondErrorMessage: string = '';
    private newQuestionModel: INewQuestionModel =
        {
            text: '',
            answers: [{
                id: 0,
                text: '',
                questionId: 0,
                isCorrect: false
            }]
        };
    private message: string = '';
    private returnUrl: string;
    private service: AnswerableService
    private questions: IQuestion[] = []
    private modelQuestions: IQuestion[] = []
    private questionToBeAdded: IQuestion | null = null

    private answerCounter: number = 1

    constructor(@IRouter private router: IRouter, private httpClient: HttpClient, private appState: AppState) {
        this.service = new AnswerableService(httpClient, appState)
    }

    async load(params: Params) {
        const response = await this.service.get(params[0]);

        if (response.statusCode === 200) {
            this.model = response.data
        } else {
            this.errorMessage = response.errorMessage
        }
    }

    async attached() {
        const questionService = new QuestionService(this.httpClient, this.appState)
        const response = await questionService.getAll();
        if (response.statusCode === 200) {
            this.questions = response.data
        } else {
            this.errorMessage = response.errorMessage
        }

        await this.getAllQuestions();
    }

    async getAllQuestions() {
        const newResponse = await this.service.getAllQuestionsForAnswerable(this.model.id)
        if (newResponse.statusCode === 200) {
            this.modelQuestions = newResponse.data
        } else {
            this.errorMessage = newResponse.errorMessage
        }
    }

    async onSubmit() {
        this.errorMessage = '';
        if (!this.validate()) return

        const response = await this.service.update(this.model.id, { ...this.model, type: this.model.type.toString() === EType.Quiz.toString() ? 0 : 1 });

        if (response.statusCode === 204) {
            this.message = "Saved!"
        } else {
            this.errorMessage = response.errorMessage
        }
    }

    validate() {
        if (!this.model.name) {
            this.errorMessage = "Name is required."
            return false
        }
        if (!this.model.description) {
            this.errorMessage = "Description is required."
            return false
        }
        return true
    }

    backToList() {
        this.router.load("/answerable-router/")
    }

    async addExistingQuestion() {
        if (!this.questionToBeAdded) {
            this.errorMessage = "Please select a question!"
            return
        }
        if (this.model.type != this.questionToBeAdded.type) {
            this.errorMessage = "Question must be of correct type!"
            return
        }
        this.errorMessage = ''
        const response = await this.service.addExistingQuestion(this.questionToBeAdded.id, this.model.id);
        if (response.statusCode === 204) {
            this.message = "Added!"
            await this.getAllQuestions();
        } else {
            this.errorMessage = response.errorMessage
        }
    }

    viewExistingQuestion() {
        if (!this.questionToBeAdded) return
        this.router.load(`answerable-router/question-view(${this.questionToBeAdded.id})`)
    }

    async addNewQuestion() {
        if (this.newQuestionModel.text === '') {
            this.secondErrorMessage = "Add question text!"
        } else if (this.newQuestionModel.answers.length < 2) {
            this.secondErrorMessage = "Question must have at least 2 answers!"
        } else if (this.model.type === EType.Quiz && this.newQuestionModel.answers.filter(x => x.isCorrect).length !== 1) {
            this.secondErrorMessage = "Quiz must have 1 correct answer!"
        } else if (this.newQuestionModel.answers.filter(x => x.text === '').length !== 0) {
            this.secondErrorMessage = "No Answer can be empty!"
        }
        else {
            this.secondErrorMessage = ""
            const type = this.model.type === 0 ? 0 : 1
            const questionId = await this.createQuestion(this.newQuestionModel.text, type)
            if (questionId === null) return
            await this.service.addExistingQuestion(questionId, this.model.id);
            await this.createAnswers(questionId, this.newQuestionModel.answers)
            this.newQuestionModel =
            {
                text: '',
                answers: [{
                    id: 0,
                    text: '',
                    questionId: 0,
                    isCorrect: false
                }]
            };
            this.answerCounter = 1
            this.message = "Created!"
            await this.getAllQuestions();
        }
    }

    async createAnswers(questionId: number, answerList: IAnswer[]) {
        const service = new QuestionService(this.httpClient, this.appState);
        for (let answer of answerList) {
            await service.addAnswer(questionId, { text: answer.text, isCorrect: answer.isCorrect } as IAnswerCreate)
        }
    }

    async createQuestion(text: string, type: number): Promise<number | null> {
        const service = new QuestionService(this.httpClient, this.appState);
        const response = await service.create({ text: text, type: type });
        if (response.statusCode === 201) {
            return response.data.id
        } else {
            this.secondErrorMessage = response.errorMessage
            return null
        }
    }

    increaseAnswerCounter() {
        this.answerCounter++;
        this.newQuestionModel.answers.push({
            id: 0,
            text: '',
            questionId: 0,
            isCorrect: false
        })
    }

    removeAnswer(index: string) {
        document.getElementById(index).remove()
        this.answerCounter--;
        this.newQuestionModel.answers.splice(parseInt(index), 1)
    }

    async removeQuestion(questionId: number) {
        const response = await this.service.removeQuestion(this.model.id, questionId);
        if (response.statusCode === 204) {
            this.message = "Removed question!"
            await this.getAllQuestions();
        } else {
            this.errorMessage = response.errorMessage
        }
    }
}