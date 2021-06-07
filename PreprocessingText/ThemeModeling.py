#VARIABLES
top_tokens=[]
model

###########################
#bigartm - тематическое моделирование
################################
def themeModeling():
    conn = psycopg2.connect(dbname='db_urls', user='postgres', 
                        password='password', host='192.168.56.129')
    cursor = conn.cursor()

    cursor.execute('SELECT id, b_text	FROM public.preprocessing')
    records = cursor.fetchall()
    import io
    f=io.open('datas.txt','w',encoding='utf8')
    for row in records:
        f.write(row[1])
    f.close()

    import os
    if not (os.path.exists('modelsOfBA')):
        os.mkdir("modelsOfBA")

    import artm
    # создание частотной матрицы из batch
    batch_vectorizer = artm.BatchVectorizer(data_path='datas.txt',# путь к "мешку слов"
                                            data_format='vowpal_wabbit',# формат данных
                                           target_folder='modelsOfBA', #папка с частотной матрицей из batch
                                            batch_size=len(os.listdir(path))+1)# количество документов водном batch
    dictionary = artm.Dictionary(data_path='modelsOfBA')# загрузка данных в словарь
    global model
    model = artm.ARTM(num_topics=len(os.listdir(path))+1,
                      num_document_passes=10,#10 проходов по документу
                      dictionary=dictionary,
                      scores=[artm.TopTokensScore(name='top_tokens_score')])
    model.fit_offline(batch_vectorizer=batch_vectorizer, num_collection_passes=10)#10 проходов по коллекции
    global top_tokens
    top_tokens = model.score_tracker['top_tokens_score']

    for topic_name in model.topic_names:
        print (topic_name)
        for (token, weight) in zip(top_tokens.last_tokens[topic_name],
                                   top_tokens.last_weights[topic_name]):    
             print (token, '-', round(weight,3))
    #############################################
    ###############################################


usefullSites=[]
################################################################
##################Сравнение с исходным документом################
################################################################
def checkwithOriginal():
    # пока исходный документ последний
    
    for i in range(1,len(model.topic_names)-1):
        counter=0
        for (token, weight) in zip(top_tokens.last_tokens[model.topic_names[i]],
                                   top_tokens.last_weights[model.topic_names[i]]):    
             ####сравнение 
             for (tokenO, weightO) in zip(top_tokens.last_tokens[model.topic_names[-1]],
                                   top_tokens.last_weights[model.topic_names[-1]]):    
                 if token==tokenO:
                    counter+=1
        print("coincidence for ",model.topic_names[i]," is ", counter/10, "%")
        usefullSites.append(counter)
    #################################################################


################################################################
##################Определение полезных документов################
################################################################
def findUsefull():
    conn = psycopg2.connect(dbname='db_urls', user='postgres', 
                            password='password', host='192.168.56.129')
    cursor = conn.cursor()
    cursor.execute('SELECT id, url,visit_count	FROM public.cs_rawdata')
    records = cursor.fetchall()
    for id in range(len(usefullSites)):
        if(usefullSites[id]>1 and records[id][2]>1):#хороший сайт если много совпадающий популярных слов и частая посещаемость студентами
            print(records[id][1], " is usefull site")
    cursor.close()
    conn.close()
    ##############################################