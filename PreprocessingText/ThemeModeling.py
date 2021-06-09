#VARIABLES
top_tokens=[]
id_of_css=[]
model
#varibale to rem
id_of_req

###########################
#bigartm - тематическое моделирование
################################
def themeModeling():
    conn = psycopg2.connect(dbname='db_urls', user='postgres', 
                        password='password', host='192.168.56.129')
    cursor = conn.cursor()

    reques, time_p=""
    global id_of_req
    #
    # считываем задание на 
    #
    cursor.execute('SELECT id_of_req ,name_disp, text_of_disp, time_of_para   FROM public.requests INNER JOIN public.discipline ON requests.id_disp=discipline.id_disp')
    recordsGET=cursor.fetchall()
    for row in recordsGET:
        reques=row[2]
        time_p=row[3]
        id_of_req=row[0]
        cursor.execute('DELETE FROM public.requests WHERE id_of_req='+row[0])
        break#выход после первой!!!!!!!   
    #
    #####format the original text
    ##########################
    lines = (line.strip() for line in reques.splitlines())#разделение текста на строки
    chunks = (phrase.strip() for line in lines for phrase in line.split("  "))#разделение строк на куски текста
    reques = '\n'.join(chunk for chunk in chunks if (chunk and len(chunk)>15))##добавление строк в reques, если фраза длинее 15 символов    

    stop_words_ru= set(nltk.corpus.stopwords.words('russian'))
    stop_words_en= set(nltk.corpus.stopwords.words('english'))#добавить АНГ.ЯЗ

    word=nltk.word_tokenize(reques)# токинезация текста i-го документа
    word_ws=[w.lower()  for w in   word if w.isalpha() ]#нижний регистр
    word_w=[w for w in word_ws if w not in stop_words_ru ]#исключение слов и символов

    lem = mystem.lemmatize ((" ").join(word_w))# лемматизация текста
    lema=[w for w in lem if w.isalpha() and len(w)>1]
    freq=nltk.FreqDist(lema)# распределение слов в i -м документе по частоте
        
    z=[] #обновление списка для нового документа
    z=[(key+":"+str(val)) for key,val in freq.items() if val>1] # частота упоминания через : от слова
    reques=("|text" +" "+(" ").join(z)+"\n")
    #########################
    ########################
    
    str_for_time=''
    for t in time_p.split(' '):
        if(t[1]==1 and t[3]==1 and t[5]==1):
            str_for_time+= 'last_visit_time<'+ '2021-01-04 10:35:00' + ' and last_visit_time>' +'2021-01-04 09:00:00'#время первной недели и первой пары в понедельник 9 января
        elif(t[1]==1 and t[3]==1 and t[5]==2):
            str_for_time+= 'last_visit_time<'+ '2021-01-04 10:45:00' + ' and last_visit_time>' +'2021-01-04 12:20:00'#время первной недели и 2 пары в понедельник 9 января
        # и далее

    #cursor.execute('SELECT preprocessing.id, b_text,id_cs	FROM public.preprocessing INNER JOIN public.cs_rawdata ON preprocessing.id_cs=cs_rawdata.id  WHERE '+str_for_time)
    cursor.execute('SELECT preprocessing.id, b_text,id_cs	FROM public.preprocessing INNER JOIN public.cs_rawdata ON preprocessing.id_cs=cs_rawdata.id')# !!!добавить фильтр по time_p
    records = cursor.fetchall()
    import io
    f=io.open('datas.txt','w',encoding='utf8')
    global id_of_css
    for row in records:
        f.write(row[1])
        id_of_css.append(row[2])
    f.write(reques)# добавляем на анализ ИСХОДНЫЙ ТЕКСТВ
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
    global top_tokens
    global model
    global id_of_css

    for i in range(1,len(model.topic_names)-1):
        counter=0
        for (token, weight) in zip(top_tokens.last_tokens[model.topic_names[i]],
                                   top_tokens.last_weights[model.topic_names[i]]):    
             ####сравнение 
             for (tokenO, weightO) in zip(top_tokens.last_tokens[model.topic_names[-1]],
                                   top_tokens.last_weights[model.topic_names[-1]]):    
                 if token==tokenO:
                    counter+=1
        conn = psycopg2.connect(dbname='db_urls', user='postgres', 
                                    password='password', host='192.168.56.129')
        cursor = conn.cursor()
        cursor.execute('INSERT INTO public.output_usefull (percentage, id_req, id_cs) VALUES ('+ counter/10*100 +', '+ id_of_req +', '+ id_of_css[i] +' )')
        cursor.close()
        conn.close()
        #print("coincidence for ",model.topic_names[i]," is ", counter/10*100, "%")
        #usefullSites.append(counter)
    #################################################################


################################################################
##################Определение полезных документов################
################################################################
#def findUsefull():
#    conn = psycopg2.connect(dbname='db_urls', user='postgres', 
#                            password='password', host='192.168.56.129')
#    cursor = conn.cursor()
#    cursor.execute('SELECT visit_count,percentage,url,id_cs	FROM public.cs_rawdata INNER JOIN public.output_usefull ON cs_rawdata.id=output_usefull.id_cs')
#    records = cursor.fetchall()
#    for row in records:
#        if(row[0]>1 and row[1]>30):#хороший сайт если много совпадающий популярных слов и частая посещаемость студентами
#            print(records[id][1], " is usefull site")
#    cursor.close()
#    conn.close()
#    ##############################################