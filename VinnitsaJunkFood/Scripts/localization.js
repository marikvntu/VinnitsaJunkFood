//0-English
//1 - Russian
//2 - Ukrainian
var langId =0;

var dictionary = {
    addOutletNormal: ["New outlet", "Новая точка", "Нова точка"],    
    addComment: ["Add comment", "Добавить комментарий", "Додати комментарій"],
    feebackTitle: ["Feedback", "Обратная связь", "Зворотній звя'зок"],
    send: ["Send", "Отправить", "Відправити"],
    cancel: ["Cancel", "Отмена", "Відміна"],
    subject: ["Subject", "Тема", "Тема"],
    subjectTitle: ["Describe the topic in a few words", "Опиши тему несколькими словами", "Опиши тему декількома словами"],
    contactInfo: ["Your e-mail", "Твоя почта", "Твоя пошта"],
    contactInfoTitle: ["If you want my response, please specify your e-mail", "Если ты хочешь узнать мой ответ на твой отзыв, оставь пожалуйста свой e-mail", "Якщо ти хочеш дізнатись мою відповідь на твій відгук, залиш будь-ласка e-mail"],
    feedbackMessage: ["Message", "Сообщение", "Повідомлення"],
    feedbackMessageTitle: ["Please specify the problem, mistake on the site, suggestion or anythying else you would like to tell me", "Пожалуйста укажи замечания, ошибку на сайте, предложение или что-нидудь еще, все что бы Вы хотелось сказать", "Будь-ласка вкажіть зауваження, помилку на сайті, пропозицію або щось-інще, щоб Ви хотіли сказати"],
    required: ["Required field", "Обязательное поле", "Обовя'зкове поле"],
    emptyFeedbackMessage: ["Empty feedback message, the feedback was not sent.", "Пустое сообщение, отзыв не отправлен", "Порожнє повідомлення, відгук не відправлений"],
    feedbackSuccess: ["Your feedback successfully sent to", "Твой отзыв успешно отправлен на", "Твій відгук успішно відправлено на "],
    feedbackFailed: ["Could not send your feedback to", "Ошибка при отправке твоего отзыва на", "Помилка при відправленні твого відгуку на"],
    priceFormat: ["Enter price as whole numbers or 1-2 decimal digits, e.g.: ", "Введи цену в целым числом или 1-2 знаками после запятой, напр.: ", "Введи ціну як ціле число, або з 1-2 знаками після коми напр. : "],
    rating: ["Rating of quality: ", "Рейтинг качества: ", "Рейтинг якості: "],
    rated: [" user(s) rated)", " пользователь(ей) оценили)", " користувач(ів) оцінили)"],
    clickPin: ["Click the pin for more details", "Нажми булавку для деталей", "Натисни булавку для деталей"],
    selectOutlet: ["You must select an outlet","Тебе надо выбрать точку","Тобі необхідно вибрати точку"],
    canAddAssortment : ["You can now add meals from assortment to the pricelist", "Теперь ты можешь добавлять блюда с ассортимента в прайс", "Тепер ти можеш додавати страви з асортименту в прайс"],
    deleteExistingRows: ["You cannot delete existing rows, please contact administrator", "Ты не можеш удалять существующие позиции, обратись к администратору", "Ти не можеш видаляти існуючі позиції, будь-ласка звернись до адміністратора"],
    minOutletName: ["Please enter at least 3 characters in outlet name", "Пожалуйста введи минимум 3 символа в имя торговой точки", "Будь-ласка введи мінімум 3 символи в імя торгової точки"],
    outletDescriptionMaxLen: ["Outlet description may not be more than 2000 characters", "Описание торговой точки не может быть больше 2000 символов", "Опис торгової точки не може бути довшим за 2000 символів"],
    ratingConstraint: ["Please enter numeric value to rating or leave blank", "Пожалуйста введи численое значение рейтинга или оставь пустым", "Будь-ласка введи числове значення рейтингу чи залиш порожнім"],
    commentLength: ["Nickname and Comment have to contain more than 3 characters", "Ник и комментарий не могут быть меньше 3 символов", "Нік, або коментарій не може бути менше символів"],
    selectOutlet : ["You must first set the location of the outlet on a map","Ты сперва должен обозначить нахождение точки на карте","Ти спочатку повинен позначити місцезнаходження точки на карті"]
};
