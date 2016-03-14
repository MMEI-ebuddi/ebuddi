# encoding: utf-8
require "logstash/filters/base"
require "logstash/namespace"
require "logstash/timestamp"
require "json"


# Add any asciidoc formatted documentation here
# This example filter will replace the contents of the default
# message field with whatever you specify in the configuration.
#
# It is only intended to be used as an example.
class LogStash::Filters::SPlitData < LogStash::Filters::Base

  # Setting the config_name here is required. This is how you
  # configure this filter from your Logstash config.
  #
  # filter {
  #   example { message => "My message..." }
  # }
  config_name "split_userdata"

  # Replace the message with this value.
  config :target, :validate => :string, :default => "@message"
  config :source, :validate => :string, :default => "message"
  config :ts_field, :validate => :string, :default => "date_time"

  def createsig(body)
    Digest::MD5.hexdigest(  body )
  end



  public
  def register
    require "nokogiri"
    require "xmlsimple"
    # Add instance variables
  end # def register

  def processSession(v)


      correctedAnswers = Array.new
      wrongdAnswers = Array.new
      correctedActions = Array.new 
      wrongedActions = Array.new 

      allActions = Array.new
      quizQuestions = Array.new



      run = v['run'] 


      noOfRuns = run.size


      totalTimeForRun = 0

      run.each do |r|
        actions = r['actions']
        rno = r['number']
        begin
          actions = actions[0]['action']
        rescue Exception => e
          actions = []
        end

        quizQuestions = r['quizQuestions']
        begin
          quizQuestions = quizQuestions[0]['question']
        rescue Exception => e
          quizQuestions = []
        end

        quizQuestions = quizQuestions.nil? ? [] : quizQuestions
        actions = actions.nil? ? [] : actions

        



        actions.each do |a|
          answer = a['answer']
          expected = a['expected']
          a['run_no'] = rno;

          totalTime = Integer(a['totalTime'])

          if totalTime > totalTimeForRun
            totalTimeForRun = totalTime
          end


          if answer == expected
            correctedActions.push(answer)
          else
            wrongedActions.push(answer)
          end
        end

        allActions << actions

        

      end

      quizQuestions.each do |q|
        answer = q['answers']
        expected = q['correctAnswer']
        text = q['text']

        if answer == expected
          correctedAnswers.push(text)
        else
          wrongdAnswers.push(text)
        end
      end





      v.delete('run')


      # v["actions"] = allActions
      ## v["correctActions"] = correctActions
      # v["quizQuestions"] = quizQuestions

      wrongedActions.uniq!
      correctedActions.uniq!
      correctedAnswers.uniq!
      wrongdAnswers.uniq!

      v["wrongedActions"] = wrongedActions
      v["correctedActions"] = correctedActions
      v["correctedAnswers"] = correctedAnswers
      v["wrongdAnswers"] = wrongdAnswers
      v["totalTimeForRun"] = totalTimeForRun
      v["noOfRuns"] = noOfRuns
      v["wrongAnswersCount"] = wrongAnswersCount = wrongdAnswers.length
      v["wrongActionsCount"] = wrongedActions.length

      donningCombinedScore = 0
      doffingCombinedScore = 0

      if v["type"] == "Scene1" then
        donningCombinedScore = noOfRuns - 2 + wrongAnswersCount
      end

      if v["type"] == "Scene3" then
        doffingCombinedScore = noOfRuns - 2 + wrongAnswersCount
      end

      v["doffingCombinedScore"] = doffingCombinedScore
      v["donningCombinedScore"] = donningCombinedScore

      

  end

  def processQuestion(question)
    answers = question["answers"]
    correctAnswer = question["correctAnswer"]
    question["answers"] = question["answers"].split(",")

    question["time"] = Integer(question["time"])
    question["isCorrect"] = (answers == correctAnswer)

  end

  def processAction(anew)
    expected = anew["expected"]
    answer = anew["answer"]

    anew["isCorrect"] = (expected == answer)

    anew["totalTime"] = Integer(anew["totalTime"])
    anew["time"] = Integer(anew["time"])
  end

  public
  def split_events(userdata)

    
    split_events = []
    user  = userdata["User"][0]
    userId = user["uniqueUserId"] || user["name"]
    user_sex = user["sex"]
    user_name = user["name"]
    user_age = Integer(user["age"])

    usernew = {}
    
    usernew["computed_id"] = createsig(userId)
    usernew["type"] = "user"
    usernew["date_time"] = Time.now.strftime("%d/%m/%Y %H:%M:%S %p")

    userinfo = {}
    userinfo["age"] = user_age
    userinfo["name"] = user_name
    userinfo["sex"] = user_sex
    userinfo["userId"] = userId

    usernew = usernew.merge(userinfo)

    session = userdata["session"]



    unless session.nil?
      session.each do |s|
        next if s.empty?

        session_info = {}

        session_type = s["type"]
        date_time = s["date"]
        s.delete('date')
        session_id = createsig(session_type + date_time + userId)
        location = { lat: s["latitude"] , lon: s["longitude"] }

        if s["latitude"] == "0" then
          location = { lat: 8.488317 , lon: -9.771055 }
        end
        

        session_info["sessionType"]  = session_type
        session_info["date_time"]  = date_time
        session_info["sessionId"]  = session_id
        session_info["location"]  = location
        session_info["trainer"]  = s["trainer"]
        session_info["hospital"]  = s["hospital"]

        session_info = session_info.merge(userinfo)

        # s["sessionId"] = session_id

        run = s["run"]

        unless run.nil?

          run.each do |r|
            next if r.empty?
            

            run_number = r["number"]

            run_info = {}

            run_info["runNumber"] = run_number

            run_info = run_info.merge(session_info)

            actions = r["actions"]

            unless actions.nil?

              action = actions[0]["action"] || nil 

              unless action.nil?

                action.each_with_index do  |a,index|
                  _type = "action"
                  anew = { }
                  anew["type"] = _type
                  anew["computed_id"] = createsig(_type + userId + session_id + run_number + date_time + index.to_s)

                  anew = anew.merge(a)
                  anew = anew.merge(run_info)

                  processAction(anew)

                  

                  split_events << anew
                end
                
              end
              
            end

            quizQuestions = r["quizQuestions"]

            unless quizQuestions.nil?

              question = quizQuestions[0]["question"] || nil

              unless question.nil?

                question.each_with_index do |q,index|
                  _type = "question"
                  anew = {}
                  anew["type"] = _type
 
                  anew["computed_id"] = createsig(_type + userId + session_id + run_number + date_time + index.to_s)


                  anew = anew.merge(q)
                  anew = anew.merge(run_info)

                  

                  processQuestion(anew)

                  split_events << anew
                end

                
              end
              
            end

          end
          
        end

        processSession(s)

        s["type"] = "session"
        s["computed_id"] = session_id
        s = s.merge(session_info)

        split_events << s

      end
      
    end


    split_events << usernew
    # split_events << userdata

    split_events
  end

  public
  def filter(event)

    value = event[@source]


    evt = JSON.parse(value)

    value = evt["message"]

    xmlparsed = nil
    userdata = nil

    begin
      xmlparsed = XmlSimple.xml_in(value)
      userdata = xmlparsed['userdata']
      
    rescue Exception => e
      puts "Got error #{e}"
      
    end

    


    splitted_events = []
    if userdata
      userdata.each do |udd|
        splitted_events_n = split_events(udd)
        splitted_events.concat splitted_events_n
      end
      

    splitted_events.each do |ud|
      next if ud.empty?


      event_split = event.clone
      # event_split[@target] = ud
      event_split[@source] = nil
      ev_type = ud["type"] || "file_logs"

      if ev_type == "file_logs"
        event_split[@target] = ud 
      else
        ud.each do |k,v|
          event_split[k] = v
        end
        # event_split = event_split.merge(ud)
      end



      filter_matched(event_split)

      yield event_split
    end
  end

    event.cancel 
  end # def filter

end # class LogStash::Filters::Example